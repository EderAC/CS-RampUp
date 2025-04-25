using TaskManagerApp.Models;
using TaskManagerApp.Services;
using System.Text.Json;

namespace Tests.src.Services
{
    public class TaskFileStorageTests
    {
        private readonly TaskFileStorage _storage;
        private readonly string _testFilePath;

        public TaskFileStorageTests()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), "tasks_test.json");
            _storage = new TaskFileStorage(_testFilePath);
        }

        [Theory]
        [MemberData(nameof(GetTaskData))]
        public void SaveTasksToFile_ShouldSaveTasksToFile(List<UserTask> tasks)
        {
            try
            {

            // Act
            _storage.SaveTasksToFile(tasks);

            // Assert
            Assert.True(File.Exists(_testFilePath), "The file was not created");
            var savedTasks = JsonSerializer.Deserialize<List<UserTask>>(File.ReadAllText(_testFilePath));
            Assert.NotNull(savedTasks);
            Assert.Equal(tasks.Count, savedTasks!.Count);
            Assert.Equal(tasks[0].Name, savedTasks[0].Name);
            }
            finally
            {
                // Clean up
                if(File.Exists(_testFilePath)){
                    File.Delete(_testFilePath);
                }
            }
        }
    
        [Theory]
        [MemberData(nameof(GetTaskData))]
        public void LoadTasksFromFile_ShouldLoadTasksFromFile(List<UserTask> tasks)
        {
            // Arrange
            File.WriteAllText(_testFilePath, JsonSerializer.Serialize(tasks));

            // Act
            try
            {

                Assert.True(File.Exists(_testFilePath), "The file was not created");
                var recoveredTasks = _storage.LoadTasksFromFile();
                
                Assert.NotNull(recoveredTasks);
                Assert.Equal(recoveredTasks.Count, tasks.Count);
                Assert.Equal(recoveredTasks[0].Name, tasks[0].Name);
            }
            finally
            {
                // Clean up
                if(File.Exists(_testFilePath)){
                    File.Delete(_testFilePath);
                }
            }
        }

        // Test data provider
        public static IEnumerable<object[]> GetTaskData()
        {
            yield return new object[]
            {
                new List<UserTask>
                {
                    new ("Test Task 1", DateTime.Now, TaskPriority.Low)
                    {
                        Id = 1,
                        Description = "Description 1",
                        Status = TaskManagerApp.Models.TaskStatus.ToDo
                    },
                    new ("Test Task 2", DateTime.Now, TaskPriority.Low)
                    {
                        Id = 2,
                        Description = "Description 2",
                        Status = TaskManagerApp.Models.TaskStatus.Completed
                    }
                }
            };

            yield return new object[]
            {
                new List<UserTask>
                {
                    new ("Single task", DateTime.Now, TaskPriority.High) 
                    {
                        Id = 1,
                        Description = "Description 1",
                        Status = TaskManagerApp.Models.TaskStatus.ToDo
                    }
                }
            };
            yield return new object[]
            {
                new List<UserTask> {
                    new ("Only obligatory data task", DateTime.Now, TaskPriority.High)
                }
            };

        }
    }
}
