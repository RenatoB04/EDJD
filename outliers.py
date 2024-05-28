import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns

file_path = r'C:\Users\Legion\Documents\P01-PE.csv'
data = pd.read_csv(file_path)

gender_map = {0: 'Masculino', 1: 'Feminino'}
data['Género'] = data['Género'].map(gender_map)

data_filtered = data[['Depressão', 'Género']].dropna()

def identify_outliers(series):
    Q1 = series.quantile(0.25)
    Q3 = series.quantile(0.75)
    IQR = Q3 - Q1
    lower_bound = Q1 - 1.5 * IQR
    upper_bound = Q3 + 1.5 * IQR
    outliers = series[(series < lower_bound) | (series > upper_bound)]
    return outliers

outliers_total = identify_outliers(data_filtered['Depressão'])
outliers_by_gender = data_filtered.groupby('Género')['Depressão'].apply(identify_outliers)

plt.figure(figsize=(12, 6))
sns.boxplot(x='Género', y='Depressão', data=data_filtered)

plt.figure(figsize=(12, 6))
sns.histplot(data=data_filtered, x='Depressão', hue='Género', kde=True, element='step')
plt.show()