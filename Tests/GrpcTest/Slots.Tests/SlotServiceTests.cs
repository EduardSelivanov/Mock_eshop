using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using SlotsService;
using SlotsService.Data;
using SlotsService.Models;
using SlotsService.Repos;
using SlotsService.Services;

namespace Slots.Tests
{
    public class SlotServiceTests
    {
        //private DbContextOptions<SlotsContext> GetDbOptions()
        //{
        //    string someDBName = $"DBName+{Guid.NewGuid().ToString()}";
        //    return new DbContextOptionsBuilder<SlotsContext>()
        //        .UseInMemoryDatabase(someDBName).Options;
        //}

        private ServerCallContext GetTestCont(string MethodName)
        {
            var cont = TestServerCallContext.Create
                (
                method: MethodName,
                host: null,
                deadline: DateTime.UtcNow.AddMinutes(5),
                requestHeaders: null,
                cancellationToken: default,
                peer: null,
                authContext: null,
                contextPropagationToken: null,
                writeHeadersFunc: _ => Task.CompletedTask,
                writeOptionsGetter: ()=> null,
                writeOptionsSetter: _ => { }
                );

            return cont;
        }

        [Fact]
        public async Task AssignProductToSlot_Ok_WhenFreeSlot()
        {
            Mock<ISlotRepo> mockRepo = new Mock<ISlotRepo>();
            Guid id = Guid.NewGuid();
            SlotModel freeSlot = new SlotModel { SlotId = id, IsFree = true };

            mockRepo.Setup(repp => repp.GetFreeSlot()).ReturnsAsync(freeSlot);

            SlotService service = new SlotService(mockRepo.Object);

            AssignProdReq req = new AssignProdReq()
            {
                SKU = "TEST_SKU",
                Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow)
            };

            AssignProdResp resp = await service.AssignProductToSlot(req, GetTestCont("AssignProductToSlot"));

            Assert.True(resp.Success);

            mockRepo.Setup(repo => repo.GetSlotByID(id)).ReturnsAsync(freeSlot);

            Assert.NotNull(freeSlot);
            Assert.False(freeSlot.IsFree);
            Assert.Equal("TEST_SKU", freeSlot.SKU);
        }

        [Fact]
        public async Task EditRackAndMoveProds_Ok_WhenFreeSlots()
        {
            Mock<ISlotRepo> mockRepo = new Mock<ISlotRepo>();
            

            Guid onRackId = Guid.NewGuid();

            Guid freeSlotID = Guid.NewGuid();
            Guid freeSlotID2 = Guid.NewGuid();
            List<SlotModel> freeSlots = new List<SlotModel>()
            {
                new SlotModel()
                {
                    SlotId=freeSlotID,
                    IsFree=true
                    ,OnRackId=Guid.NewGuid()
                },
                new SlotModel()
                {
                    SlotId=freeSlotID2,
                    IsFree=true,
                    OnRackId=Guid.NewGuid()
                }
            };

            Guid slotToMove = Guid.NewGuid();
            Guid slotToMove2 = Guid.NewGuid();
            DateOnly date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
            List<SlotModel> slotsToMove = new List<SlotModel>()
            {
                new SlotModel()
                {
                    SlotId=slotToMove,
                    SKU="Move to other",
                    ArriveDate=date,
                    OnRackId=onRackId,
                    IsFree=false,
                    X=1,
                    Y=2
                },
                new SlotModel()
                {
                    SlotId=slotToMove2,
                    SKU="Move other to other",
                    ArriveDate=date,
                    OnRackId=onRackId,
                    IsFree=false,
                    X=2,
                    Y=2
                }
            };

            mockRepo.Setup(repo => repo.GetSlotsByRack(It.IsAny<Guid>(),false))
           .ReturnsAsync(slotsToMove);

            mockRepo.Setup(repo => repo.GetFreeSlots(It.IsAny<bool>()))
                    .ReturnsAsync(freeSlots);

            mockRepo.Setup(repo => repo.RemoveSlots(It.IsAny<List<SlotModel>>()))
                    .Returns(Task.CompletedTask);


            SlotService slotService = new SlotService(mockRepo.Object);

            EditRackReq req = new EditRackReq()
            {
                RackId = onRackId.ToString(),
                MaxX = 1,
                MaxY=1
            };

            EditRackResp resp = await slotService.EditRackAndMoveProds(req,GetTestCont("EditRackAndMoveProds"));

            Assert.True(resp.Succes);
            Assert.Equal("Move to other", freeSlots[0].SKU);
        }

        [Fact]
        public async Task EditRackAndMoveProds_NotOk_WhenNoFreeSlots()
        {
            Mock<ISlotRepo> mockRepo = new Mock<ISlotRepo>();

            Guid onRackId = Guid.NewGuid();
            Guid slotToMoveId = Guid.NewGuid();
            List<SlotModel> slotsToMove = new List<SlotModel>()
            {
                new SlotModel
                {
                SlotId = slotToMoveId,
                SKU = "Move to other",
                ArriveDate = DateOnly.FromDateTime(DateTime.Now),
                OnRackId = onRackId,
                IsFree = false,
                X = 1,
                Y = 2
                }
            };

            mockRepo.Setup(repo => repo.GetSlotsByRack(onRackId, false)).ReturnsAsync(slotsToMove);

            mockRepo.Setup(repo => repo.GetFreeSlots(true)).ReturnsAsync(new List<SlotModel>());

            SlotService slotService = new SlotService(mockRepo.Object);

            EditRackReq req = new EditRackReq()
            {
                RackId = onRackId.ToString(),
                MaxX = 1,
                MaxY = 1
            };
            EditRackResp resp = await slotService.EditRackAndMoveProds(req, GetTestCont("EditRackAndMoveProds"));

            Assert.False(resp.Succes);
        }

    }
}
