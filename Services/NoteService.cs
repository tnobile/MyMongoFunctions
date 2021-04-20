using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoFunctions.Helpers;
using MyNotes.Functions.Models;

namespace MyMongoFunctions.Services
{
    // https://willvelida.medium.com/creating-a-web-api-with-azure-functions-azure-cosmos-db-mongodb-api-and-c-9bd8d528405a
    public interface INoteService
    {
        Task CreateNote(Note note);
        Task<Note> GetNote(string id);
        Task<List<Note>> GetNotes();
        Task<DeleteResult> RemoveNote(Note note);
        Task<DeleteResult> RemoveNoteById(string id);
        Task<ReplaceOneResult> UpdateBook(string id, Note note);
    }

    public class NoteService : INoteService
    {
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Note> _notes;
        public NoteService(MongoClient mongoClient, IConfiguration configuration)
        {
            _mongoClient = mongoClient;
            _database = _mongoClient.GetDatabase(configuration[Settings.DATABASE_NAME]);
            _notes = _database.GetCollection<Note>(configuration[Settings.COLLECTION_NAME]);
        }

        public async Task CreateNote(Note note)
        {
            await _notes.InsertOneAsync(note);
        }

        public async Task<Note> GetNote(string id)
        {
            var book = await _notes.FindAsync(note => note.Id == id);
            return book.FirstOrDefault();
        }

        public async Task<List<Note>> GetNotes()
        {
            return await _notes.Find(book => true).ToListAsync();
        }

        public async Task<DeleteResult> RemoveNote(Note note)
        {
            return await this.RemoveNoteById(note.Id);
        }

        public async Task<DeleteResult> RemoveNoteById(string id)
        {
            return await _notes.DeleteOneAsync(book => book.Id == id);
        }

        public async Task<ReplaceOneResult> UpdateBook(string id, Note note)
        {
            return await _notes.ReplaceOneAsync(n => n.Id == id, note);
        }
    }
}