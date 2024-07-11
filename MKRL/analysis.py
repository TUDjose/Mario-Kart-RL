import os
import pandas as pd
import matplotlib.pyplot as plt
os.environ['TF_ENABLE_ONEDNN_OPTS'] = '0'


# Read Analytics file
file_path = r'Analytics/analytics_20240710_233308.txt'
df = pd.read_csv(file_path, sep=';', header=None)
df.columns = ['ID', 'FinishedLap', 'EpisodeLength', 'Rewards', 'Lesson']
df['ID'] = df['ID'].apply(lambda x: int(x[4:]))
df['FinishedLap'] = df['FinishedLap'].apply(lambda x: bool(x))
df['EpisodeLength'] = df['EpisodeLength'].apply(lambda x: int(x))
df['Rewards'] = df['Rewards'].apply(lambda x: float(x.replace(',', '.')))
df['Lesson'] = df['Lesson'].apply(lambda x: int(x))

fastest_lap = df[df['FinishedLap'] is True].sort_values(by='EpisodeLength').iloc[0]['EpisodeLength'] * 0.02
total_step_count = df['EpisodeLength'].sum()
num_episodes = len(df)
percent_finished = len(df[df['FinishedLap'] is True]) / num_episodes

print(f'Fastest Lap: {fastest_lap} seconds',
      f'Total Step Count: {total_step_count}',
      f'Number of Episodes: {num_episodes}',
      f'Percent Finished: {percent_finished * 100:.1f}%',
      sep='\n')


# Get Tensorboard csv files
cumulative_rewards = pd.read_csv(r'Analytics/rewards.csv', header=0).iloc[:, 1:]

