using LangLang.Repositories;
using System;
using System.Collections.Generic;
using LangLang.Models;

namespace LangLang.Services;
public class PenaltyPointService : IPenaltyPointService
{
    private readonly IPenaltyPointRepository _penaltyPointRepository;
    
    public PenaltyPointService(IPenaltyPointRepository penaltyPointRepository)
    {
        _penaltyPointRepository = penaltyPointRepository;
    }

    public List<PenaltyPoint> GetAll()
    {
        return _penaltyPointRepository.GetAll();
    }

    public PenaltyPoint? GetById(int id)
    {
        return _penaltyPointRepository.GetById(id);
    }
    public void Add(PenaltyPointReason penaltyPointReason, int studentId, int courseId, int teacherId, DateOnly datePenaltyPointGiven)
    {
        PenaltyPoint point = new(penaltyPointReason, false, studentId, courseId, teacherId, datePenaltyPointGiven);
        _penaltyPointRepository.Add(point);
    }
    public void Delete(int id)
    {
        _ = _penaltyPointRepository.GetById(id) ?? throw new InvalidInputException("PenaltyPoint doesnt exist.");
        _penaltyPointRepository.Delete(id);
    }
}
