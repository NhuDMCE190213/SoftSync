using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories;

public interface IMentorRepository : IRepository<Mentor> { }
public class MentorRepository : Repository<Mentor>, IMentorRepository
{
    public MentorRepository(Data.SoftSyncDbContext context) : base(context) { }
}
