const WebSocket = require('ws');
const wss = new WebSocket.Server({
    port: 3000
});

const rooms = {
    room1: {}
}


wss.on('connection', function connection(ws) {

    ws.on('message', function incoming(json) {
         const arg = JSON.parse(json)

         switch (arg.action){
            case 'connect':
                onJoin(arg)
                break;
            case 'disconnect':
                onLeave(arg)
                break;
            case 'move':
                onMove(arg)
                break;
            case 'stop':
                onStop(arg)
                break;
            default:
                break;
         }
    });

    ws.on('close', () => {
        delete rooms['room1'][ws.user]
    })

    function onJoin(arg) {
        rooms['room1'][arg.user] = arg

        sendAll(rooms)

        waitAction(arg.user);
    }

    function onLeave(arg) {

        waitAction(arg.user);

        delete rooms['room1'][arg.user];

        sendAll(rooms);
    }

    function onMove(arg) {
        rooms['room1'][arg.user] = arg

        sendAll(rooms)

        waitAction(arg.user);
    }

    function onStop(arg){
        rooms['room1'][arg.user] = arg

        sendAll(rooms)

        waitAction(arg.user);
    }


    function waitAction(user) {
        rooms['room1'][user].action = 'wait'
    }


    function sendAll(message) {
        wss.clients.forEach(client => {
            client.send(JSON.stringify(message))
        })
    }

})