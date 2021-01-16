# EleGantt

![image](https://user-images.githubusercontent.com/6802086/104728402-26297100-5737-11eb-95d5-e20f0aa5fda2.png)

Qu'est-ce qu'EleGantt ?

EleGantt est un outil de gestion des tâches, il permet de planifier dans le temps les différentes tâches et étapes importantes (milestones) liées à un projet, son but est de rester très minimaliste afin de garder dans l'application que les actions éssentielles utilisées dans la majortiés des cas.

EleGantt se veut simple et intuitif, l'essentiel et rien d'autre.

## Présentation de l'application

### Bandeau latéral
Depuis le bandeau latéral, il y a possible de gérer entirement le projet, premièrement lors de la création d'un nouveau projet nous pouvons définir 3 paramètres importants : le nom du projet, la date de début ainsi que la date de fin

![image](https://user-images.githubusercontent.com/6802086/104786970-b7780200-578e-11eb-8219-09c7cd68394b.png)

Les dates de début et de fin sont importantes car elles vont limiter la création de tâche et de miltestone dans cet intervalle.

Dans la suite du bandeau, se trouve la gestion des tâches ainsi que les différentes actions disponibles dans EleGantt

![image](https://user-images.githubusercontent.com/6802086/104790604-f8284900-5797-11eb-8452-90d2edde17cf.png)

Les 3 boutons sont respectivement : la suppression d'une tâche, l'ajout d'un milestone ainsi que l'ajout d'une tâche.

La liste des tâches dans le bandeau permet un rapide coup d'oeil sur toutes les tâches en évitant de devoir dérouler la timeline afin de trouver la tâche en question. 

![image](https://user-images.githubusercontent.com/6802086/104787572-1f7b1800-5790-11eb-9ac1-31a009605e84.png)

La liste permet de faire de la multi-sélection ainsi on peut déplacer un groupe de tâche à différents endroits. Elle peut aussi être conjointement utilisée avec le bouton supprimer afin d'enlever plusieurs tâches en même temps. Il est aussi possible d'utiliser directement la touche "delete" afin de supprimer une tâche.

Un double clique ou un "F2" permet d'editer directement et facilement le nom de la tâche 

![image](https://user-images.githubusercontent.com/6802086/104787770-b942c500-5790-11eb-8a19-fd430e520796.png)

### Timeline

La timeline permet d'avoir une vue d'ensemble des tâches ains que des étapes importantes du projet répartis sur le temps.

![image](https://user-images.githubusercontent.com/6802086/104788648-0cb61280-5793-11eb-9499-18edd4ada3da.png)

Elle peut être manipulée avec plusieurs contrôles, par exemple avec une CTRL + molette de la souris, il est possible de dézoomer et de zoomer sur la time line.

![image](https://user-images.githubusercontent.com/6802086/104790399-54d73400-5797-11eb-807a-c162358f7d9d.png)

Pour revenir à l'état initial il suffit de cliquer sur la molette.

Lorsque l'on effectue un double clique sur les tâches ou sur une miletsone, une popup apparait afin de permettre l'édition de ses caractériques.

![image](https://user-images.githubusercontent.com/6802086/104789345-19d40100-5795-11eb-9ca8-c5904780e17d.png)\
_Edition d'une tâche_

![image](https://user-images.githubusercontent.com/6802086/104789393-412ace00-5795-11eb-9e4e-95bc5faefe1b.png)\
_Edition d'un milestone_

Ces popups permettent de modifier rapidement les dates de début et de fin. Lorsqu'on souhaite déplacer une tâche et non modifier sa durée il est possible de simplement "drag & droper" la tâche. De cette manière on peut faire glisser la tâche à n'importe quel endroit de la timeline, la même chose est possible pour les milestones.

### Menu 

Depuis le menu plusieurs actions sont possibles :

* Créer un nouveau projet
* Ouvrir un projet existant
* Sauvegarder rapidement (sur le dernier chemin connu) le projet en cours
  * Peut être activée via CTRL + S
* Sauvegarder le projet dans chemin spécifique
* Export de la timeline en : 
  * PNG
  * dans le presse-papiers
* Changement du thème, deux thème sont disponibles, un "dark" theme ainsi qu'un "light" theme.

![image](https://user-images.githubusercontent.com/6802086/104789843-5fdd9480-5796-11eb-9419-899869b5108f.png)
_Light theme_

#### Modification en cours

Lorsque des modifications (ajout de tâche ou de milestones) ont été effecutées et qu'elles n'ont pas été sauvegardées, la présence d'une "\*" dans le tirtre de l'application vous l'annonce.

![image](https://user-images.githubusercontent.com/6802086/104790201-2d806700-5797-11eb-9160-181465a6c705.png)

## Librairies utilisées

* [Gong WPF Dragdrop](https://github.com/punker76/gong-wpf-dragdrop)
* [Material Design In XAML Toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
* [MathConverter](https://github.com/hexinnovation/MathConverter)
* [Newtonsoft.Json](https://www.newtonsoft.com/json)
* [System.Windows.Interactivity.WPF](https://www.nuget.org/packages/System.Windows.Interactivity.WPF/)
