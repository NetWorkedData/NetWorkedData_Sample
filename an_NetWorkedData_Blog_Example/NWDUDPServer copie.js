var PORT = 7777;
var HOST = '0.0.0.0';

var dgram = require('dgram');
var server = dgram.createSocket('udp4');

var BattleList = { };
var UserList = { };
server.on('listening', function() 
  {
    var address = server.address();
    console.log('UDP Server listening on ' + address.address + ':' + address.port);
  }
);
server.on('error', function(message) 
  {
    console.log('Error : ' + message);
  }
);
var LastSend = 0
server.on('message', function(message, remote) 
  {
        let tError = false;
        // console.log(remote.address + ':' + remote.port +' - ' + message);
        let BattleReceipt  = JSON.parse(message);
        let ObjectSend = 
        { 
          R : '',
          W : '',
        }

        let Listener = {
          IP : '0.0.0.0',
          Port : PORT,
          Players : '',
        }

        let Battle = 
        {
            BattleReccord : {
              R : BattleReceipt.R, // reference of Battle (string)
              T : new Date(), // date of this update (date time UTC)
              S : BattleReceipt.S, //statut of Battle  (int)
              D : 'no', // string debug
              I : 0, // increment of send
              E : new Array(), // players (every players) informations
              O : new Array(), // objects or ennemies informations (only changed)
            },
            IP : new Array(), // players IP and 
        }
        
        let BattleSend = 
        {
          R : BattleReceipt.R,
          T : new Date(),
          S : BattleReceipt.S,
          // P : 
          // {
          //   R : '',
          //   S : '',
          //   W : '',
          // },
          // A :
          // { 
          //   R : BattleReceipt.A.R,
          //   S : BattleReceipt.A.S,
          //   W : BattleReceipt.A.W,
          // },
          D : 'no',
          I : 0,
          Y : 0,
          // C : new Array(),
          E : new Array(),
          IP : new Array(),
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
          BattleSend.Y = BattleReceipt.Y
          // update player in arena
          let index = -1;
          if (BattleReceipt.P.R !='' && BattleReceipt.P.W !='')
          {
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
            tError = true;
            // console.log(remote.address + ':' + remote.port +' - ' + message);
            BattleSend.E.push(BattleReceipt.P.W)
          }
        }
        if(BattleSend.IP.includes(remote.address+':'+remote.port)==false)
        {
          BattleSend.IP.push(remote.address+':'+remote.port);
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
        // const data =Buffer.from(message)

       let hrTime = new Date()
        // console.log("udp_server", "send ? ", hrTime)
        if (hrTime-LastSend >=1000/60)
        {
          BattleSend.IP.forEach( IPPort => 
            { 
              // console.log("udp_server", "will send", IPPort)
          let IPPortArray = IPPort.split(':')
          // console.log("udp_server", "will send", IPPortArray)

          LastSend = hrTime
          BattleSend.T = hrTime
          const data = Buffer.from(JSON.stringify(BattleSend))
          // console.log("udp_server", "send", "YES")
          // server.send(data, remote.port, remote.address, (error, bytes) => 
          server.send(data, IPPortArray[1], IPPortArray[0], (error, bytes) => 
          {
            if(error)
            {
              console.log("udp_server", "error", 'error sent to ' +  IPPortArray[0] + IPPortArray[1])
              console.log("udp_server", "error", error)
              // client.close()
            } 
            else 
            {
              //console.log("udp_server", "info", 'Data sent to ' +  IPPortArray[0] + IPPortArray[1])
            }    
          }
        )
        })
      }
        if (tError==true)
        {
          console.log("new insert ")
          console.log("")
          console.log(remote.address + ':' + remote.port +' - ' + message);
          //console.log("udp_server", "info", 'Data sent !' + data)
        }
  }
);

server.bind(PORT, HOST);