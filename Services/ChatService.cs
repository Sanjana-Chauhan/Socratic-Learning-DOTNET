namespace SOCRATIC_LEARNING_DOTNET.Services;

using Microsoft.EntityFrameworkCore;
using SOCRATIC_LEARNING_DOTNET.Interfaces;
using SOCRATIC_LEARNING_DOTNET.Entities;
using SOCRATIC_LEARNING_DOTNET.Data;

// Services/ChatService.cs
public class ChatService : IChatService
{
    private readonly AppDbContext _db;

    public ChatService(AppDbContext db) => _db = db;

    public async Task<Conversation> GetOrCreateConversationAsync(string userId, Guid? convId)
    {
        if (convId.HasValue)
            return await _db.Conversations.FindAsync(convId.Value)
                ?? throw new Exception("Not found");

        var c = new Conversation
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Title= "New Conversation"
        };
        _db.Conversations.Add(c);
        await _db.SaveChangesAsync();
        return c;
    }

    public async Task<Message> AddMessageAsync(Message msg)
    {
        _db.Messages.Add(msg);
        await _db.SaveChangesAsync();
        return msg;
    }

    public Task<IEnumerable<Conversation>> ListConversationsAsync(string userId) =>
        _db
            .Conversations.Where(c => c.UserId == userId)
            .OrderByDescending(c => c.UpdatedAt)
            .AsNoTracking()
            .ToListAsync()
            .ContinueWith(t => (IEnumerable<Conversation>)t.Result);

    public Task<IEnumerable<Message>> GetMessagesAsync(Guid conversationId) =>
        _db
            .Messages.Where(m => m.conversationId == conversationId)
            .OrderBy(m => m.timestamp)
            .AsNoTracking()
            .ToListAsync()
            .ContinueWith(t => (IEnumerable<Message>)t.Result);
}
