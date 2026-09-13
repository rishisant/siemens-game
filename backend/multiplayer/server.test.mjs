import {test} from 'node:test';
import assert from 'node:assert/strict';
import {once} from 'node:events';
import {WebSocket} from 'ws';
import {createGameServer} from './server.mjs';
const waitFor=(ws,predicate)=>new Promise((resolve,reject)=>{
 const timeout=setTimeout(()=>{ws.off('message',handler);reject(Error('message timeout'));},3000);
 const handler=data=>{const message=JSON.parse(data);if(predicate(message)){clearTimeout(timeout);ws.off('message',handler);resolve(message);}};ws.on('message',handler);
});
test('Two players share a room, movement and chat; rooms are isolated and cleaned up',async()=>{
 const app=createGameServer({port:0,host:'127.0.0.1'});await once(app.server,'listening');
 const connect=async()=>{const ws=new WebSocket(`ws://127.0.0.1:${app.server.address().port}/socket`);await once(ws,'open');return ws;};
 try{
  const a=await connect(),b=await connect(),c=await connect();
  let response=waitFor(a,p=>p.type==='welcome');a.send(JSON.stringify({type:'create',name:'Rishi'}));const host=await response;
  response=waitFor(b,p=>p.type==='error');b.send(JSON.stringify({type:'join',room:'BAD123'}));assert.match((await response).message,/not found/);
  response=waitFor(b,p=>p.type==='welcome');b.send(JSON.stringify({type:'join',room:host.room,name:'Friend'}));await response;
  response=waitFor(c,p=>p.type==='welcome');c.send(JSON.stringify({type:'create',name:'Other room'}));await response;
  response=waitFor(b,p=>p.type==='snapshot'&&p.players.some(v=>v.name==='Rishi'&&v.x===12));
  a.send(JSON.stringify({type:'state',scene:'Town_Square',x:12,y:20,moving:true,facing:'Right',equipped:[108]}));const snapshot=await response;assert.equal(snapshot.players.length,2);
  response=waitFor(b,p=>p.type==='chat'&&p.message==='hello');a.send(JSON.stringify({type:'chat',message:'hello'}));assert.equal((await response).name,'Rishi');
  const other=await waitFor(c,p=>p.type==='snapshot');assert.equal(other.players.length,1);
  response=waitFor(a,p=>p.type==='snapshot'&&p.players.length===1);b.close();await response;
  a.close();c.close();await new Promise(r=>setTimeout(r,50));assert.equal(app.rooms.size,0);
 } finally {await app.close();}
});

test('Automatic town entry puts friends together without a room code',async()=>{
 const app=createGameServer({port:0,host:'127.0.0.1'});await once(app.server,'listening');
 try {
  const clients=[];
  for(let i=0;i<9;i++) {
   const ws=new WebSocket(`ws://127.0.0.1:${app.server.address().port}/socket`);await once(ws,'open');clients.push(ws);
   const response=waitFor(ws,p=>p.type===(i<8?'welcome':'error'));
   ws.send(JSON.stringify({type:'auto',name:`Friend ${i}`}));
   const packet=await response;if(i<8)assert.equal(packet.room,'TOWN01');else assert.match(packet.message,/8 players/);
  }
  assert.equal(app.rooms.size,1);assert.equal(app.rooms.get('TOWN01').size,8);
  for(const ws of clients)ws.close();await new Promise(r=>setTimeout(r,50));assert.equal(app.rooms.size,0);
 } finally {await app.close();}
});
