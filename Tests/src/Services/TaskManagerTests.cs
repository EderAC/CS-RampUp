using Moq;
using TaskManagerApp.Interfaces;
using TaskManagerApp.Models;
using TaskManagerApp.Services;

namespace Tests.src.Services
{
    public class TaskManagerAppTests
    {
        private readonly Mock<ITaskFileStorage> _mockStorage;
        private readonly List<UserTask> _initialTasks;
        private readonly TaskManager _taskManager;

        public TaskManagerAppTests()
        {
            _initialTasks = new List<UserTask>();
            _mockStorage = new Mock<ITaskFileStorage>();
            _mockStorage.Setup(s => s.LoadTasksFromFile()).Returns(_initialTasks);
            _taskManager = new TaskManager(_mockStorage.Object);
        }

        [Fact]
        public void AddTask_ShouldAssignIdAndSave()
        {
            // Arrange
            var task = new UserTask ("New task", DateTime.Now.AddDays(1), TaskPriority.Medium);

            // Act
            _taskManager.AddTask(task);

            // Assert
            Assert.Equal(1, task.Id);
            Assert.Single(_taskManager.GetAllTasks());
            _mockStorage.Verify(s => s.SaveTasksToFile(It.IsAny<List<UserTask>>()), Times.Once);
        }

        [Fact]
        public void DeleteTask_ShouldRemoveTaskAndSave()
        {
            // Arrange
            var task = new UserTask ("New task", DateTime.Now.AddDays(1), TaskPriority.Medium)
                { 
                    Id = 1,
                    Status = TaskManagerApp.Models.TaskStatus.ToDo
                };
            _initialTasks.Add(task);

            // Act
            var result = _taskManager.DeleteTask(1);

            // Assert
            Assert.True(result);
            Assert.Empty(_taskManager.GetAllTasks());
            _mockStorage.Verify(s => s.SaveTasksToFile(It.IsAny<List<UserTask>>()), Times.Once);
        }

        [Fact]
        public void DeleteTask_TaskNotFound_ShouldThrow()
        {
            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _taskManager.DeleteTask(99));
            Assert.Contains("Task not found", ex.Message);
        }

        [Fact]
        public void UpdateTaskStatus_ShouldUpdateStatusAndSave()
        {
            // Arrange
            var task = new UserTask ("New task", DateTime.Now.AddDays(1), TaskPriority.Medium)
                {
                    Id = 1,
                    Status = TaskManagerApp.Models.TaskStatus.ToDo
                };

            _initialTasks.Add(task);

            // Act
            var result = _taskManager.UpdateTaskStatus(1, TaskManagerApp.Models.TaskStatus.Completed);

            // Assert
            Assert.True(result);
            Assert.Equal(TaskManagerApp.Models.TaskStatus.Completed, task.Status);
            _mockStorage.Verify(s => s.SaveTasksToFile(It.IsAny<List<UserTask>>()), Times.Once);
        }

        [Theory]
        [InlineData(TaskManagerApp.Models.TaskStatus.ToDo)]
        [InlineData(TaskManagerApp.Models.TaskStatus.InProgress)]
        public void GetTasksByStatus_ShouldReturnMatchingTasks(TaskManagerApp.Models.TaskStatus status)
        {
            // Arrange
            _initialTasks.AddRange(
            [
                new UserTask ("New task", DateTime.Now.AddDays(1), TaskPriority.Medium) { Id = 1, Status = status },
                new UserTask ("New task", DateTime.Now.AddDays(1), TaskPriority.Medium) { Id = 2, Status = TaskManagerApp.Models.TaskStatus.Completed }
            ]);

            // Act
            var result = _taskManager.GetTasksByStatus(status);

            // Assert
            Assert.All(result, t => Assert.Equal(status, t.Status));
        }

        [Fact]
        public void GetNextTasks_ShouldReturnUpcomingToDoTasks()
        {
            // Arrange
            _initialTasks.AddRange(
            [
                new UserTask ("New task", DateTime.Now.AddDays(2), TaskPriority.Medium) { Id = 1, Status = TaskManagerApp.Models.TaskStatus.ToDo},
                new UserTask ("New task", DateTime.Now.AddDays(1), TaskPriority.Medium) { Id = 2, Status = TaskManagerApp.Models.TaskStatus.Completed},
                new UserTask ("New task", DateTime.Now.AddDays(-1), TaskPriority.Medium) { Id = 3, Status = TaskManagerApp.Models.TaskStatus.ToDo }
            ]);

            // Act
            var result = _taskManager.GetNextTasks();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
        }

        [Fact]
        public void GetNextTasks_NoTasks_ShouldThrow()
        {
            // Arrange: All tasks are past due or not ToDo
            _initialTasks.Add(
                new UserTask ("New task", DateTime.Now.AddDays(1), TaskPriority.Medium)
                { 
                    Status = TaskManagerApp.Models.TaskStatus.Completed 
                }
            );

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _taskManager.GetNextTasks());
            Assert.Equal("No tasks found", ex.Message);
        }
    }
}