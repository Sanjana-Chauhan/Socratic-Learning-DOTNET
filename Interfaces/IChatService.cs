namespace SOCRATIC_LEARNING_DOTNET.Interfaces;

using SOCRATIC_LEARNING_DOTNET.Entities;
// Interfaces/IChatService.cs
public interface IChatService
{
    Task<Conversation> GetOrCreateConversationAsync(string userId, Guid? convId);
    Task<Message> AddMessageAsync(Message msg);
    Task<IEnumerable<Conversation>> ListConversationsAsync(string userId);
    Task<IEnumerable<Message>> GetMessagesAsync(Guid conversationId);
}
