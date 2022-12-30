 private void ConfigureJacobApp(IServiceCollection services)
        {
            services.AddSingleton<JobController>();
            services.AddSingleton<IJobControllerSettings, JobControllerSettings>();
            services.AddSingleton<IJobScheduleService, JobScheduleService>();
        }

        private async void ConfigureQuartz(IServiceCollection services)
        {
            var container = services.BuildServiceProvider();
            var jobFactory = new JobFactory(container);

            try
            {
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

                IScheduler scheduler = await schedulerFactory.GetScheduler();

                scheduler.JobFactory = jobFactory;

                IJobDetail jobController = JobBuilder.Create<JobController>()
                    .WithIdentity("jobController", "jobControllerGroup")
                    .Build();

                ITrigger jobControllerTrigger = TriggerBuilder
                    .Create()
                    .WithIdentity("jobControllerTrigger", "jobControllerTriggerGroup")
                    .WithSimpleSchedule(x => x.WithIntervalInHours(1))
                    .ForJob(jobController)
                    .Build();

                bool doesJobControllerExist = await scheduler.CheckExists(jobController.Key);

                if (doesJobControllerExist)
                {
                    await scheduler.Start();
                }
                else
                {
                    if (!doesJobControllerExist)
                    {
                        await scheduler.ScheduleJob(jobController, jobControllerTrigger);
                    }

                    await scheduler.Start();
                }
            }
            catch
            {
                throw;
            }
        }
