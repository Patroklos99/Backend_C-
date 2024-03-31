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
    
    }
}