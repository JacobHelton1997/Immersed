public interface IJobScheduleService
    {
        int Add(JobScheduleAddRequest model, int userId);
        void Update(JobScheduleUpdateRequest model, int userId);
        JobSchedulesPending GetAllPending();
        void Delete(int id, int userId);
    }
