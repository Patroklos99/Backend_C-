namespace Project.messages.model;

public record Message(
    string id,
    string username,
    long timestamp,
    string text,
    string imageUrl);