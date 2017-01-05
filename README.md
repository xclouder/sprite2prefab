# sprite2prefab
unity3d里根据sprite生成prefab的工具

使用方法：
1)clone仓库到工程Assets/Editor目录下: 
clone https://github.com/xclouder/sprite2prefab.git SpriteTools

2)修改SpriteTools/config
```
{
  "tasks":
  {
    "data_folder": "Assets的相对目录",
    "prefab_folder": "Assets的相对目录"
  }
}
```

3)点击Unity菜单： `Assets/GeneratePrefab_for_Sprite` 开始生成
