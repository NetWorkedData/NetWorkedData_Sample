var PORT = 7777;
var HOST = '0.0.0.0';

var dgram = require('dgram');
var server = dgram.createSocket('udp4');

var BattleList = { };
server.on('listening', function() 
  {
    var address = server.address();
    console.log('UDP Server listening on ' + address.address + ':' + address.port);
  }
);

server.on('message', function(message, remote) 
  {
        //console.log(remote.address + ':' + remote.port +' - ' + message);
        let BattleReceipt  = JSON.parse(message);
        let ObjectSend = 
        { 
          R : '',
          W : '',
        }
        let BattleSend = 
        {
          R : BattleReceipt.R,
          S : BattleReceipt.S,
          P : 
          {
            R : '',
            S : '',
            W : '',
          },
          A :
          { 
            R : BattleReceipt.A.R,
            S : BattleReceipt.A.S,
            W : BattleReceipt.A.W,
          },
          D : 'no',
          I : 0,
          Y : 0,
          C : new Array(),
          E : new Array(),
          O : new Array()
          // M : message.toString(),
        }

        // find the goog battle on server
        if ((BattleReceipt.R in BattleList))
          {
            BattleSend = BattleList[BattleReceipt.R]
            BattleSend.D = 'Allready in list'
            BattleSend.I = BattleSend.I+1
          }
        else
          {
            BattleList[BattleReceipt.R] = BattleSend
            BattleSend.D = 'New in list'
            BattleSend.I = 0
            BattleSend.E = new Array()
          }
          // client counter
          BattleSend. Y = BattleReceipt.Y
          // update player in arena
          let index = -1;
        if (Array.isArray(BattleSend.E))
          {
            BattleSend.E.forEach(element => 
              { 
              if (element.includes(BattleReceipt.P.R))
                {
                  index = BattleSend.E.indexOf(element)
                } 
              }
            );
          }
        else
          {
            BattleSend.E = new Array()
          }
        if (index>-1)
          {
            BattleSend.E.splice(index,1,BattleReceipt.P.W)
          }
        else
          {
            BattleSend.E.push(BattleReceipt.P.W)
          }

        // update object in arena
        BattleReceipt.C.forEach( ObjectInteractable => 
          { 
            ObjectSend = JSON.parse(ObjectInteractable);
            let indexB = -1;
            if (Array.isArray(BattleSend.O))
              {
                BattleSend.O.forEach(
                  ObjectInteracted => { 
                    if (ObjectInteracted.includes(ObjectSend.R))
                      {
                        indexB = BattleSend.O.indexOf(ObjectInteracted)
                      } 
                  });
              }
            else
              {
                BattleSend.O = new Array()
              }
            if (indexB > -1)
              {
                BattleSend.O.splice(indexB,1,ObjectInteractable);
              }
            else
              {
                BattleSend.O.push(ObjectInteractable)
              }
          }
        )
        //sending msg
        const data = Buffer.from(JSON.stringify(BattleSend))
        // const data =Buffer.from(message)
        server.send(data, remote.port, remote.address, (error, bytes) => 
          {
            if(error)
            {
              console.log("udp_server", "error", error)
              client.close()
            } 
            else 
            {
              console.log("udp_server", "info", 'Data sent !!!')
            }    
          }
        )
  }
);

server.bind(PORT, HOST);