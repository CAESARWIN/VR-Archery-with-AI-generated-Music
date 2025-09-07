import matplotlib.pyplot as plt
import pandas as pd

# 假设你已经有两个列表（每个22个评分，对应每个被试的沉浸度评分）
# ✅ 替换下面的分数为你自己的真实数据
scores_fixed = [5, 4, 5, 6, 4, 4, 6, 5, 4, 5, 4, 4, 4, 3, 3, 4, 3, 5, 4, 3, 5, 4]      # 固定音乐
scores_adaptive = [5, 6, 5, 5, 5, 5, 5, 6, 5, 5, 6, 5, 6, 6, 4, 5, 6, 6, 5, 5, 6, 5]  # 自适应音乐

# 构建 DataFrame
df_fixed = pd.DataFrame({
    'Immersion Score': scores_fixed,
    'Condition': ['Fixed Music'] * 22
})

df_adaptive = pd.DataFrame({
    'Immersion Score': scores_adaptive,
    'Condition': ['Adaptive Music'] * 22
})

# 合并数据
df_all = pd.concat([df_fixed, df_adaptive], ignore_index=True)

# 绘图
plt.figure(figsize=(6, 4))
box = df_all.boxplot(column='Immersion Score', by='Condition', patch_artist=True)

# 设置颜色
colors = ['lightpink', 'lightblue']
for patch, color in zip(box.artists, colors):
    patch.set_facecolor(color)
    patch.set_edgecolor('black')

# 图形设置
plt.title('')  # 去掉默认标题
plt.suptitle('')  # 去掉分组标题
plt.ylabel('Immersion Score')
plt.xlabel('')
plt.ylim(0, 8)  # 保证 y 轴覆盖评分区间 1-7
plt.grid(axis='y', linestyle='--', alpha=0.7)
plt.tight_layout()
plt.show()
