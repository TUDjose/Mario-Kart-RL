import os
import pandas as pd
import matplotlib.pyplot as plt
os.environ['TF_ENABLE_ONEDNN_OPTS'] = '0'


# Read Analytics file
# file_path = r'Unity/analytics_20240711_145126.txt'
# file_path = r'Unity/analytics_20240717_225726.txt'
file_path = r'Unity/analytics_20240718_163200.txt'
df = pd.read_csv(file_path, sep=';', header=None)
df.columns = ['ID', 'FinishedLap', 'EpisodeLength', 'Rewards', 'Lesson', 'Step']
df['ID'] = df['ID'].apply(lambda x: int(x[4:]))
df['FinishedLap'] = df['FinishedLap'].apply(lambda x: bool(x))
df['EpisodeLength'] = df['EpisodeLength'].apply(lambda x: int(x))
df['Rewards'] = df['Rewards'].apply(lambda x: float(x.replace(',', '.')))
df['Lesson'] = df['Lesson'].apply(lambda x: int(x))
df['Step'] = df['Step'].apply(lambda x: int(x))


# Get some statistics
fastest_lap = df[df['FinishedLap'] == True].sort_values(by='EpisodeLength').iloc[0]['EpisodeLength'] * 0.02
total_step_count = df['EpisodeLength'].sum()
num_episodes = len(df)
percent_finished = len(df[df['FinishedLap'] == True]) / num_episodes
better_than_human = len(df[(df['FinishedLap'] == True) & (df['EpisodeLength'] < 1870)]) / len(df[(df['FinishedLap'] == True)])

print(f'Fastest Lap: {fastest_lap} seconds',
      f'Total Step Count: {total_step_count}',
      f'Number of Episodes: {num_episodes}',
      f'Percent Finished: {percent_finished * 100:.1f}%',
      f'Better than human laps: {better_than_human * 100:.1f}%',
      sep='\n')


# Get Tensorboard csv files
# cumulative_rewards = pd.read_csv(r'Unity/cumulative_reward.csv', header=0).iloc[:, 1:]
# plt.plot(cumulative_rewards.iloc[:, 0], cumulative_rewards.iloc[:, 1])
# plt.vlines(x=[5e5, 2e6, 5e6], ymin=-5, ymax=17, colors='r', linestyles='dashed', lw=1)
# plt.xlabel('Step')
# plt.ylabel('Cumulative Reward')
# plt.grid()
# plt.tight_layout()
# plt.savefig('cumulative_rewards.png', dpi=300)
# plt.show()

# episode_lengths = pd.read_csv(r'Analytics/episode_length.csv', header=0).iloc[:, 1:]
# plt.plot(episode_lengths.iloc[:, 0], episode_lengths.iloc[:, 1])
# plt.vlines(x=[5e5, 2e6, 5e6], ymin=0, ymax=2100, colors='g', linestyles='dashed', lw=1)
# plt.xlabel('Step')
# plt.ylabel('Episode Length [steps]')
# plt.grid()
# plt.show()


def lr_sensitivity():
      for i in range(1, 8):
            cumulative_rewards = pd.read_csv(rf'lrs/lr_{i}_MarioKartRL.csv', header=0).iloc[:, 1:]
            plt.plot(cumulative_rewards.iloc[:, 0], cumulative_rewards.iloc[:, 1])
            # plt.vlines(x=[5e5, 2e6, 5e6], ymin=-5, ymax=17, colors='r', linestyles='dashed', lw=1)
      plt.legend(['$\eta$ = 5e-5', '$\eta$ = 1e-4', '$\eta$ = 3e-4', '$\eta$ = 5e-4', '$\eta$ = 8e-4', '$\eta$ = 1e-3', '$\eta$ = 5e-3'])
      plt.xlabel('Step')
      plt.ylabel('Cumulative Reward')
      plt.grid()
      plt.tight_layout()
      plt.savefig('lr_sensitivity.png', dpi=300)
      plt.show()

def epsilon_sensitivity():
      for i in range(1,7):
            cumulative_rewards = pd.read_csv(rf'epsilons/eps_{i}_MarioKartRL.csv', header=0).iloc[:, 1:]
            plt.plot(cumulative_rewards.iloc[:, 0], cumulative_rewards.iloc[:, 1])
      plt.legend(['$\epsilon$ = 0.01', '$\epsilon$ = 0.1', '$\epsilon$ = 0.2', '$\epsilon$ = 0.3', '$\epsilon$ = 0.5', '$\epsilon$ = '                                                                                                           '0.8'])
      plt.xlabel('Step')
      plt.ylabel('Cumulative Reward')
      plt.grid()
      plt.tight_layout()
      plt.savefig('epsilon_sensitivity.png', dpi=300)
      plt.show()

def gamma_sensitivity():
      for i in range(1,7):
            cumulative_rewards = pd.read_csv(rf'gammas/gamma_{i}_MarioKartRL.csv', header=0).iloc[:, 1:]
            plt.plot(cumulative_rewards.iloc[:, 0], cumulative_rewards.iloc[:, 1])
      plt.legend(['$\gamma$ = 0.5', '$\gamma$ = 0.8', '$\gamma$ = 0.9', '$\gamma$ = 0.95', '$\gamma$ = 0.99', '$\gamma$ = 1.0'])
      plt.xlabel('Step')
      plt.ylabel('Cumulative Reward')
      plt.grid()
      plt.tight_layout()
      plt.savefig('gamma_sensitivity.png', dpi=300)
      plt.show()
