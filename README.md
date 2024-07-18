# Mario-Kart-RL
AE4350 Bio-inspired intelligence and learning for aerospace applications assignment - using reinforcement learning (PPO) to create a Mario Kart like agent to complete a track

## Code description
The actual implementation of the PPO algorithm is provided by the Unity ML-Agents Toolkit, which contains a Python 
API perform training of the environment created in Unity. To run new training, the CLI method can be used (please 
refer to [Unity ML-Agents](https://github.com/Unity-Technologies/ml-agents) for further setup instructions):

```bash
mlagents-learn Assets/Configs/<config_file.yaml> --env=Builds --no_graphics
```

Nonetheless, the Agent and Environment need to be created in Unity. A build for this environment can be downloaded in 
the Releases page, where the final RL model can be found and run to observe how it performs in the training and 
testing tracks. Additionally, in this build, the tracks can also be manually attempted, to try to beat the agent's 
lap time! 

With regards to the code that makes up the agent and environment, these can be found in the `Assets/Scripts` 
directory. Most important is the `KartAgent.cs` file, which contains the implementation of the reward function and 
the agent's observation/state and actions. `TrainingManager.cs` also contains the implementation of the curriculum 
learning methods (splitting training into different phases/lessons).

## Results
This video shows the agent going through the training track in real-time (not sped up). Such speeds and accuracy are 
quite hard to match by the human controller.
![](https://github.com/TUDjose/Mario-Kart-RL/blob/main/Media/Training_track.gif)