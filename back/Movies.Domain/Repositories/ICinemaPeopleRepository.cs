using Movies.Domain.Entities;

namespace Movies.Domain.Repositories;

public interface ICinemaPeopleRepository
{
    IQueryable<CinemaPeople> GetAllCinemaPeople();
    Task<CinemaPeople?> GetCinemaPeopleById(ulong id);
    Task<CinemaPeople> AddCinemaPeople(CinemaPeople cinemaPeople);
    Task<IEnumerable<CinemaPeople>> AddRangeCinemaPeople(IEnumerable<CinemaPeople> cinemaPeople);
    CinemaPeople UpdateCinemaPeople(CinemaPeople cinemaPeople);
    IEnumerable<CinemaPeople> UpdateRangeCinemaPeople(IEnumerable<CinemaPeople> cinemaPeople);
    bool DeleteCinemaPeople(CinemaPeople cinemaPeople);
    bool DeleteRangeCinemaPeople(IEnumerable<CinemaPeople> cinemaPeople);
    Task<ulong> GetLastCinemaPeopleId();
}