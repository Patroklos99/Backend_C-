namespace Project.messages.model;

public record MessageRequest(string username, string text, ChatImageData? imageData) {
}