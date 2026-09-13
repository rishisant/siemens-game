mergeInto(LibraryManager.library, {
 BC_IsTouchDevice:function(){return navigator.maxTouchPoints>0 && window.matchMedia("(pointer: coarse)").matches ? 1 : 0;},
 BC_Connect: function(url, receiver) {
   if(window.byteCitySocket) { window.byteCitySocket.onclose=null; window.byteCitySocket.close(); }
   var name=UTF8ToString(receiver), ws=new WebSocket(UTF8ToString(url)); window.byteCitySocket=ws;
   ws.onopen=function(){SendMessage(name,'OnSocketOpen','');};
   ws.onmessage=function(e){if(window.byteCitySocket===ws)SendMessage(name,'OnSocketMessage',e.data);};
   ws.onclose=function(){if(window.byteCitySocket===ws)SendMessage(name,'OnSocketClosed','Disconnected. Join again when ready.');};
   ws.onerror=function(){if(window.byteCitySocket===ws)SendMessage(name,'OnSocketClosed','Could not connect. Check the server address.');};
 },
 BC_Send:function(message){var ws=window.byteCitySocket;if(ws&&ws.readyState===1)ws.send(UTF8ToString(message));},
 BC_Close:function(){var ws=window.byteCitySocket;window.byteCitySocket=null;if(ws)ws.close();}
});
