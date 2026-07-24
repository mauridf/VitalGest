namespace VitalGest.Application.DTOs.Schedule;

public record CreateScheduleRequest(int DoctorUserId, int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime, int SlotDuration = 30);
public record UpdateScheduleRequest(TimeOnly StartTime, TimeOnly EndTime, int SlotDuration = 30, bool IsActive = true);
public record ScheduleResponse(int Id, int DoctorUserId, string DoctorName, int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime, int SlotDuration, bool IsActive);
public record CreateScheduleExceptionRequest(int DoctorUserId, DateOnly ExceptionDate, TimeOnly? StartTime, TimeOnly? EndTime, string Reason, bool IsAvailable = false);
public record ScheduleExceptionResponse(int Id, int DoctorUserId, DateOnly ExceptionDate, string Reason, bool IsAvailable);
public record TimeSlotResponse(int Id, int DoctorUserId, string DoctorName, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, bool IsAvailable);