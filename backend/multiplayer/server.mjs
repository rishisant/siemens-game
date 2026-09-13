import http from 'node:http';
import { randomBytes, randomUUID } from 'node:crypto';
import { readFile, stat } from 'node:fs/promises';
import { resolve, extname, sep } from 'node:path';
import { fileURLToPath } from 'node:url';
import { WebSocketServer, WebSocket } from 'ws';

const scenes = new Set(['Town_Square','Laboratory_Main','Casino_Main','PipeGame','WireGame','CardJitsu','MainMenu','Tutorial','Starting-Cutscene']);
const clean = (value, max) => String(value ?? '').replace(/[\x00-\x1f<>]/g,'').trim().slice(0,max);
export function createGameServer({port=8090,host='0.0.0.0',staticRoot=resolve('../../Builds/Web')}={}) {
  const rooms=new Map();
  const server=http.createServer(async(req,res)=>{
    if(req.url==='/health'){res.writeHead(200,{'Content-Type':'application/json'});res.end(JSON.stringify({ok:true,rooms:rooms.size}));return;}
    try {
      const path=decodeURIComponent(new URL(req.url,'http://localhost').pathname);
      const file=resolve(staticRoot,'.'+(path==='/'?'/index.html':path));
      if(!file.startsWith(resolve(staticRoot)+sep))throw Error('invalid path');
      if(!(await stat(file)).isFile())throw Error('not a file');
      const mime={'.html':'text/html','.js':'application/javascript','.wasm':'application/wasm','.data':'application/octet-stream','.json':'application/json','.css':'text/css','.png':'image/png'};
      res.writeHead(200,{'Content-Type':mime[extname(file)]||'application/octet-stream','Cache-Control':'no-cache'});res.end(await readFile(file));
    } catch {res.writeHead(404);res.end('Byte City server is running. Build the Web player to serve the game here.');}
  });
  const wss=new WebSocketServer({server,path:'/socket',maxPayload:4096,perMessageDeflate:false});
  const send=(ws,payload)=>{if(ws.readyState===WebSocket.OPEN && ws.bufferedAmount<65536)ws.send(JSON.stringify(payload));};
  const broadcast=(room,payload)=>{for(const ws of room.values())send(ws,payload);};
  function leave(ws){if(!ws.room)return;const room=rooms.get(ws.room);room?.delete(ws.id);if(room?.size===0)rooms.delete(ws.room);ws.room=null;}
  wss.on('connection',ws=>{
    ws.id=randomUUID();ws.room=null;ws.alive=true;ws.lastState=0;ws.lastChat=0;ws.messages=0;ws.window=Date.now();
    const joinDeadline=setTimeout(()=>{if(!ws.room)ws.close(1008,'Join timeout');},15000);joinDeadline.unref();
    ws.on('pong',()=>ws.alive=true);
    ws.on('message',data=>{
      const now=Date.now();if(now-ws.window>1000){ws.window=now;ws.messages=0;}if(++ws.messages>40){ws.close(1008,'Too many messages');return;}
      let packet;try{packet=JSON.parse(data);}catch{send(ws,{type:'error',message:'Invalid message.'});return;}
      if(!packet||typeof packet!=='object')return;
      if(packet.type==='create'||packet.type==='join'||packet.type==='auto'){
        if(ws.room){send(ws,{type:'error',message:'Leave your current room first.'});return;}
        let code=clean(packet.room,6).toUpperCase();
        if(packet.type==='create'){
          if(rooms.size>=100){send(ws,{type:'error',message:'Server is full. Try again shortly.'});return;}
          do{code=randomBytes(4).toString('hex').slice(0,6).toUpperCase();}while(rooms.has(code));rooms.set(code,new Map());
        }
        if(packet.type==='auto'){code='TOWN01';if(!rooms.has(code)){if(rooms.size>=100){send(ws,{type:'error',message:'Server is full.'});return;}rooms.set(code,new Map());}}
        const room=rooms.get(code);
        if(!room){send(ws,{type:'error',message:'Room not found. Check the six-character code.'});return;}
        if(room.size>=8){send(ws,{type:'error',message:'This room already has 8 players.'});return;}
        ws.room=code;ws.player={id:ws.id,name:clean(packet.name,20)||'Explorer',scene:'Town_Square',x:-30.1,y:20.49,facing:'Down',moving:false,equipped:[]};room.set(ws.id,ws);
        clearTimeout(joinDeadline);send(ws,{type:'welcome',id:ws.id,room:code});
        broadcast(room,{type:'chat',name:'Byte City',message:ws.player.name+' joined the town.'});return;
      }
      if(!ws.room)return;const room=rooms.get(ws.room);
      if(packet.type==='state' && now-ws.lastState>=40){
        ws.lastState=now;
        if(!scenes.has(packet.scene)||!Number.isFinite(packet.x)||!Number.isFinite(packet.y)||Math.abs(packet.x)>500||Math.abs(packet.y)>500)return;
        ws.player={...ws.player,scene:packet.scene,x:packet.x,y:packet.y,facing:['Up','Down','Left','Right'].includes(packet.facing)?packet.facing:'Down',moving:packet.moving===true,equipped:Array.isArray(packet.equipped)?packet.equipped.filter(v=>Number.isInteger(v)&&v>=100&&v<600).slice(0,5):[]};
      }
      if(packet.type==='chat' && now-ws.lastChat>=750){
        const message=clean(packet.message,140);if(!message)return;ws.lastChat=now;broadcast(room,{type:'chat',name:ws.player.name,message});
      }
    });
    ws.on('close',()=>{clearTimeout(joinDeadline);leave(ws);});ws.on('error',()=>{});
  });
  const tick=setInterval(()=>{for(const room of rooms.values())broadcast(room,{type:'snapshot',players:[...room.values()].map(ws=>ws.player)});},100);tick.unref();
  const heartbeat=setInterval(()=>{for(const ws of wss.clients){if(!ws.alive){ws.terminate();continue;}ws.alive=false;ws.ping();}},15000);heartbeat.unref();
  server.on('close',()=>{clearInterval(tick);clearInterval(heartbeat);});
  server.listen(port,host);
  return {server,wss,rooms,close:()=>new Promise(resolveClose=>{for(const ws of wss.clients)ws.terminate();wss.close();server.close(resolveClose);})};
}
if(process.argv[1]===fileURLToPath(import.meta.url)){
 const port=Number(process.env.PORT||8090);
 createGameServer({port,staticRoot:process.env.WEB_ROOT||resolve(fileURLToPath(new URL('../../Builds/Web',import.meta.url)))});
 console.log(`Byte City multiplayer listening on ${port}`);
}
