
## Should I do this too?

Probably not. 99.999% of the time you do not want to transfer files with SignalR Core. It was built for sending many small messages. I was in a position 

When transferring a file to your web application, you're usually better off with a REST endpoint. There are a ton of examples of doing that online, so we won't talk about them here.

## What's the strategy for doing this?

The process is fairly straigtforward. The client will break apart the the file into small collections of bytes and send them individually. Once the server knows it has all pieces of the file, it will combine them into a single file.


## SignalR Hub Code

```cs



```
