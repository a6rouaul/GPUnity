# Contexte 

Dans le cadre de notre cours d’IAS de S9 à l’ENIB, nous avons travaillé sur un projet de génération procédurale de niveau d’un jeu vidéo de Platforme. Nous avons illustré ce travail en proposant un jeu nommé RunOut, dont le principe est de sortir d’un labyrinthe généré grâce à des algorithmes d'intelligence artificielle. Nous avons également profité de ce projet pour réaliser un jeu complet, jouable et exécutable à partir d’un fichier .exe. Nous avons donc finalement un jeu proposant 2 modes de jeux, correspondant à 2 manières différentes de générer le niveau du mode choisi. 
Nous avons utilisé les ressources graphiques disponibles sous licences gratuites à cette url : 
https://pixel-frog.itch.io/pixel-adventure-1

# Explications 
## Safety mode
Ce mode vous assure que le niveau généré sera correct. Ce mode utilise un algorithme sans intelligence artificielle poussée. On définit à la main un contour pour notre niveau et on définit un objet qui va se déplacer de manière aléatoire dans ce cadre. Cet objet a plusieurs contraintes: il ne peut pas revenir sur une case de la map déjà traversée, il doit s'arrêter sur la ligne la plus en bas du cadre et il ne peut pas sortir du cadre. On est dans une approche constructive car le chemin se construit au fur et à mesure de l’avancée de l’objet. Une fois que l’on a défini un chemin cohérent, on définit une liste de CellMode (enum permettant de définir les ouvertures Top, Bottom, Right, Left) pour chaque cellule de la map, qui doit permettre de traverser le chemin sans encombre. Une fois le chemin créé avec des cellules ayant un type correct, on créé de manière aléatoire toutes les autres cellules vides de la map en choisissant de manière aléatoire son CellMode.
## WFC mode 
Ce mode est basé sur l'algorithme Wave Function Collapse qui permet de générer un niveau à partir d’un seul exemple donné en entrée. Cet algorithme permet d’obtenir des niveaux parfois incroyables tout comme des niveaux très mauvais. En fonction de la figure donnée en entrée, cet algorithme va “apprendre” à identifier les différents éléments de la figure, dans notre cas des cellules de différents types mises les unes à la suite des autres. Il va ensuite créer une map avec le même type d'éléments que ceux envoyés en entrée, en s’inspirant de la logique d’associations des cellules de notre entrée.

## Pour commencer
- Télécharger Unity
  - Version de développement : Unity 2019.4.15f1
- Dans Unity
  - Window > Package Manager
  - Télécharger ‘Cinemachine’
- Cloner le projet sur votre machine [https://github.com/a6rouaul/GPUnity]
  - Ou télécharger le zip
- Ouvrez le avec Unity
  - File > Build settings…
    - (S'il y a déjà des scènes, vous pouvez les enlever et les remetter pour être sûr)
    - Ajouter MenuScene et GameSCene dans ‘Scenes in Build’
    - S’assurer que MenuScene à l’index 0 et que GameScene à l’index 1 
- Ouvrir GameScene
  - Vérifier que le Player a un Sorting Group avec :
    - Sorting Layer : Default
    - Order in Layer : 1
  - Vérifier que le EndPortal a un Sorting Group avec :
    - Sorting Layer : Default
    - Order in Layer : 0
  - Vérifier que WFCGrid a un Sorting Group avec :
    - Sorting Layer : Background (Créer cette layer et la mettre au dessus de Default dans Sorting Layers)
    - Order in Layer : -1
  - Vérifier que BackgroundGrid a un Sorting Group avec :
    - Sorting Layer : Background
    - Order in Layer : 0
  - Vérifier que LevelGrid a un Sorting Group avec :
    - Sorting Layer : Default
    - Order in Layer : 0
- Ouvrir MenuScene
- Cliquer sur ‘Play’

## Jouer à partir d’un exécutable : 
  - Allez dans le fichier Builds
  - Choisissez votre OS et lancez le jeu.
 
## Bugs encore présents :
  - Si le joueur se place entre deux tiles et qu’il n’y a qu’une tile d’espace, il restera coincé et le player sera déplacé selon l’axe Y jusqu’à ce qu’il n’y ai plus de collision.
  - BorderGrid et BackgroundGrid ne sont pas générés automatiquement. En cas de modifications de la taille de la map, il faut modifier à la main ces 2 éléments pour ne pas pouvoir sortir du niveau et pour avoir un joli fond à notre map.


