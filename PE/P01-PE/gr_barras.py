import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

data = {
    'Tempo Suficiente Sono': ['Sim', 'Não'],
    'Masculino': [20.71, 15.98],
    'Feminino': [31.36, 31.95]
}

df = pd.DataFrame(data)

df_melted = df.melt(id_vars='Tempo Suficiente Sono', var_name='Género', value_name='Frequência Relativa')

plt.figure(figsize=(10, 6))
sns.barplot(x='Tempo Suficiente Sono', y='Frequência Relativa', hue='Género', data=df_melted)

plt.xlabel('Tempo Suficiente de Sono')
plt.ylabel('Frequência Relativa (%)')

plt.show()