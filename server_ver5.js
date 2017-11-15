var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);
server.listen(3000);

var ID = function () {
  // Math.random should be unique because of its seeding algorithm.
  // Convert it to base 36 (numbers + letters), and grab the first 9 characters
  // after the decimal.
  return '_' + Math.random().toString(36).substr(2, 9);
};
function FindMyRoom(_roomKey) {
  for(var i = 0;i<roomList.length;i++){
    if(roomList[i].key===_roomKey){
      return i;
    }
  }
  return -1;
}
console.log('----running server');
var clients = [];
var roomList = [];
var playerSpawnPoints = [];
var key = 'unknown';

app.get('/',function(req,res) {
  res.send('back');
});

io.on('connection',function(socket) {

  var currentPlayer = {};
  var room ={};
  var ip = {
    address:socket.request.connection.remoteAddress,
    port:socket.request.connection.remotePort,
    name:"unknown"
  };
  currentPlayer.name = 'unknown';

  socket.on('room list',function(){
    console.log('----recv: room list');
    console.log(' emit: room list '+JSON.stringify(roomList));
    io.sockets.emit('room list',{roomList});
  })

  socket.on('create room',function(data){
    console.log(' recv: player create room, room name: '+ (data.roomName));
    key = ID();
    var members=[];
    room ={
      key: key,
      name: data.roomName,
      members:members
    }

    roomList.push(room);
    socket.emit('create room',room);
    console.log(' emit: create room '+key+', '+data.roomName);
  })

  socket.on('player connect room',function(data) {
    console.log(' recv: player connect room, current player room: '+ data.key);
    var roomIndex = FindMyRoom(data.key);
    if(roomIndex==-1)
    {
      return;
    }
    if(roomList[roomIndex].members.length>=2){
      console.log(' emit: full room '+data.key);
      socket.emit('rejected room',data);
      return;
    }
    for(var i =0;i<roomList[roomIndex].members.length;i++){
      var playerConnected ={
        name:roomList[roomIndex].members[i].name,
        position:roomList[roomIndex].members[i].position,
        rotation:roomList[roomIndex].members[i].rotation,
        room:roomList[roomIndex].members[i].room,
        socketId:roomList[roomIndex].members[i].socketId,
        role:roomList[roomIndex].members[i].role
      };
      if(playerConnected.name == data.name){
        console.log(' emit: overlap name '+data.name);
        socket.emit('rejected room',data);
        return;
      }
      socket.emit('other player connected',playerConnected);
      console.log(' emit: other player connected '+playerConnected.name+', '+playerConnected.room);
    }
    socket.emit('player connected',data);
  })

  socket.on('play',function(data) {
    console.log(' recv: play: '+JSON.stringify(data));
    var roomIndex = FindMyRoom(data.room);
    if(playerSpawnPoints.length === 0){
      data.playerSpawnPoints.forEach(function(_playerSpawnPoint){
        var playerSpawnPoint = {
          position: _playerSpawnPoint.position,
          rotation: _playerSpawnPoint.rotation
        }
        playerSpawnPoints.push(playerSpawnPoint)
      });
    }
    var role;
    if(roomList[roomIndex].members.length==0){
        role = 0;
    }
    else {
      if(roomList[roomIndex].members[0].role==0){
        role = 1;
      }
      else{
        role = 0;
      }
    }
    var randomSpawnPoint = playerSpawnPoints[role];
    currentPlayer ={
      name:data.name,
      position:randomSpawnPoint.position,
      rotation:randomSpawnPoint.rotation,
      room:data.room,
      socketId:socket.id,
      role:role
    }
    ip.name = data.name;
    roomList[roomIndex].members.push(currentPlayer);
    socket.join(data.room);
    console.log(" emit: play: "+JSON.stringify(currentPlayer));
    socket.emit('play',currentPlayer);
    socket.broadcast.to(currentPlayer.room).emit('other player connected',currentPlayer);
  })

  socket.on('player talk',function() {
    //console.log('recv: talk: '+JSON.stringify(data));
    socket.broadcast.to(currentPlayer.room).emit('player address',ip);
    //console.log(currentPlayer.name+' bcst: player talk '+JSON.stringify(currentPlayer));
  })

  socket.on('player move',function(data) {
    //console.log('recv: move: '+JSON.stringify(data));
    currentPlayer.position = data.position;
    socket.broadcast.to(currentPlayer.room).emit('player move',currentPlayer);
    //console.log(currentPlayer.name+' bcst: player move '+JSON.stringify(currentPlayer));
  })

  socket.on('player turn',function(data) {
    //console.log('recv: turn: '+JSON.stringify(data));
    currentPlayer.rotation = data.rotation;
    socket.broadcast.to(currentPlayer.room).emit('player turn',currentPlayer);
    //console.log(currentPlayer.name+' bcst: player turn '+JSON.stringify(currentPlayer));
  })

  socket.on('disconnect',function() {
    var roomIndex = FindMyRoom(currentPlayer.room);
    if(roomIndex==-1)
    {
      return;
    }
    console.log(' recv: disconnect '+currentPlayer.name +', room : '+currentPlayer.room);
    socket.leave(currentPlayer.room);
    socket.broadcast.to(currentPlayer.room).emit('other player disconnect',currentPlayer);
    console.log(' bcst: other player disconnect');
    if(roomList[roomIndex].members[0].socketId === socket.id){
      roomList[roomIndex].members.splice(0,1);
    }
    else{
      roomList[roomIndex].members.splice(1,1);
    }
    if(roomList[roomIndex].members.length === 0)
    {
      roomList.splice(roomIndex,1);
    }
    io.sockets.emit('room list',{roomList});
  })
})
