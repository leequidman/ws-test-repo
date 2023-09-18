# A basic Game Server which accepts WebSockets


## Technical details
### Server
- Server exposes one main endpoint "serverUrl/ws", all WS communication is made via this endpoint.
- If request to this endpoint is not a WS request - server responds with 400
- Communication between server and client is implemented via sending and handling events.
- Every event has a type, based on type server chooses handler(s) for this event and processes it.
- Code is organized in folders by features (Login, UpdateResource, SendGift), each folder has EventHandler, as well as EventParser and Validator.
- .Net6 is used, Serilog nuget for logging
- DI is used to provide dependencies 

Server has different layers of abstractions, such as EventHandler layer, Service layer and Repository layer.
The basic flow of processing an event is like this:
1. WS request to "serverUrl/ws" received by server
2. WebSocketHandler calls baseMessageHandler.Handle
3. BaseMessageHandler gets "EventType" from input, picks an appropriate EventParser and parses event 
4. BaseMessageHandler picks an appropriate EventHandler(s)
5. EventHandler calls some different Service methods
6. Service calls to some of Repository methods if necessary
7. Handlers sends an event with result of the operation to client

### Client
Client project is configured to communicate with server via WS
It is a ConsoleApp that has runnable "Program" class for local debugging, also it has "Client" that encapsulates events sending to server and is used in "Program" and in tests.
### Tests
Tests project uses Nunit and FluentAssertions libs, it has Functional and Unit tests
### Extendibility
To handle a new type of event it is enough to just create a model for new event (with an EventType from the enum) and a corresponding Handler that will implement IEventHandler.

It will be automatically registered in DI and will be used to handle new events

## How it was tested
1. Launched "Server" project via Vsual Studio
2. Launched "Client" project via Vsual Studio / launching tests via Tests Runner (client/tests do not launch server automatically)


## Known issues/points of improvement
- more unit tests with proper mocking (different projects to different types of tests)
- more functional tests, especially scenarios when multiple users connected or concurrent requests from one user
- setup tests infrastructure and Test Server Runner
- Dockerize server and client
- Make server port parametrizable
- true transactions in "SendGifts"
- proper handling disposed or closed WSs in 

