using CPP.API.Core;
using Microsoft.AspNetCore.Mvc;

namespace CPP.API.Controllers
{
    [Route("api/nn")]
    public class NNTrainerController : ControllerBase
    {
        private readonly INNTrainer _nnTrainer;

        public NNTrainerController(INNTrainer nnTrainer)
        {
            _nnTrainer = nnTrainer;
        }

        // UNCOMMENT TO RETRAIN THE NEURAL NETWORK
        // [HttpGet]
        // [Route("train/{epochs}")]
        // public void Train(int epochs)
        // {
        //     _nnTrainer.Train(epochs, "Data/model.bin", "Data/model2.bin");
        // }
    }
}