using AChat.Application.Common.Interfaces;
using AChat.Application.ViewModels.Note;
using AChat.Domain.Entities;
using AChat.Domain.Exceptions;
using AChat.Domain.Repositories.Base;
using AutoMapper;

namespace AChat.Application.Services;

public class NoteService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser,
    IRepositoryBase<Note> noteRepository) : BaseService(unitOfWork, mapper, currentUser)
{
    public async Task<List<NoteResponse>> GetNotesAsync(int contactId, CancellationToken ct)
    {
        var notes = await noteRepository.GetListAsync(_ => _.UserId == CurrentUser.Id && _.ContactId == contactId, ct);
        return Mapper.Map<List<NoteResponse>>(notes);
    } 
    
    public async Task<int> CreateNoteAsync(UpsertNoteRequest request, CancellationToken ct)
    {
        var note = new Note
        {
            Content = request.Content,
            ContactId = request.ContactId,
            UserId = CurrentUser.Id
        };
        
        noteRepository.Add(note);
        await UnitOfWork.SaveChangesAsync(ct);

        return note.Id;
    }
    
    public async Task UpdateNoteAsync(int id, UpsertNoteRequest request, CancellationToken ct)
    {
        var note = await noteRepository.GetByIdAsync(id, ct);
        if (note == null || note.ContactId != request.ContactId)
            throw new NotFoundException(nameof(Note), id.ToString());
        
        note.Content = request.Content;
        noteRepository.Update(note);
        await UnitOfWork.SaveChangesAsync(ct);
    }
    
    public async Task DeleteNoteAsync(int id, CancellationToken ct)
    {
        var note = await noteRepository.GetByIdAsync(id, ct);
        if (note == null || note.UserId != CurrentUser.Id)
            throw new NotFoundException(nameof(Note), id.ToString());
        
        noteRepository.Delete(note);
        await UnitOfWork.SaveChangesAsync(ct);
    }
}