import utm
import pandas as pd
import numpy as np


FilePath = 'Molly.csv' #This is the filepath to the dataset, since it and the script are in the same folder I do not need everything for the filepath
FileName = 'Molly'

FilePath = input("Please input file path: ")
FileName = input("Please give file name (This will name the created file Animal_Movement_#Name#): ")

df = pd.read_csv(FilePath, delimiter=',', engine='python', encoding='utf8', on_bad_lines='skip', parse_dates=["timestamp"], infer_datetime_format = True)
df = df.replace(np.nan, '', regex=True)
df['location-lat'] = df.apply(lambda row: row['location-lat'] if(row['location-lat'] != '') else 0, axis=1)
print(df['location-lat'])
df['location-long'] = df.apply(lambda row: row['location-long'] if(row['location-long'] != '') else 0, axis=1)
df['utm'] = df.apply(lambda row: utm.from_latlon(row['location-lat'], row['location-long']), axis=1)
utm_cols = ['easting', 'northing', 'zone_number', 'zone_letter']
for n, col in enumerate(utm_cols):
    df[col] = df['utm'].apply(lambda location: location[n])
df = df.drop('utm', axis=1)
print(df)

df.to_csv('Animal_Movement_' + FileName + '.csv', compression=None, )