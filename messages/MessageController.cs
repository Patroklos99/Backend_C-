using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Project.messages.model;
using Project.messages.repository;
using Project.WebSocket;

namespace Project.messages;

[ApiController]
[Route("/messages")] // Adjust route as necessary
public class MessageController : ControllerBase {
    private readonly IHubContext<WebSocketManagerHandler> _hubContext;
    private readonly MessageRepository _messageRepository;

    public MessageController(MessageRepository messageRepository, IHubContext<WebSocketManagerHandler> hubContext)
    {
        _messageRepository = messageRepository;
        _hubContext = hubContext;
    }

    [HttpPost]
    [Route("/messages")] 
    public async Task<IActionResult> PostMessage([FromBody] MessageRequest msgBody)
    {
        try
        {
            await _messageRepository.CreateMessage(msgBody);
            await _hubContext.Clients.All
                .SendAsync("notif"); // Replace "NotifySessions" with the actual method clients listen on
            return StatusCode(201); // HTTP 201 Created
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Unexpected error on post message: {e.Message}"); // HTTP 500 Internal Server Error
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<Message>>> GetMessages([FromQuery] string fromId = null) {
        try {
            var list = await _messageRepository.GetMessages(fromId);
            return Ok(list); // HTTP 200 OK
        }
        catch (Exception e) {
            return StatusCode(500, $"Unexpected error on get message: {e.Message}"); // HTTP 500 Internal Server Error
        }

        // Hardcoded list of messages
        // var messages = new List<Message>
        // {
        //     new("1", "UserOne", 1610000000, "Hello, this is message one!", "http://example.com/image1.jpg"),
        //     new("2", "UserTwo", 1610000001, "Here's message two, everyone.", "http://example.com/image2.jpg"),
        //     new("3", "UserThree", 1610000002, "Message three reporting in.", "http://example.com/image3.jpg"),
        //     new("4", "UserFour", 1610000003, "Fourth message, coming through!",
        //         "http://example.com/image4.jpg"),
        //     new("5", "UserFive", 1610000004, "Fifth and final message.", "http://example.com/image5.jpg")
        // };
        //
        // // Since the data is hardcoded and doesn't involve asynchronous operations,
        // // Task.FromResult is used to wrap the result in a Task.
        // return Ok(await Task.FromResult(messages));
    }
}