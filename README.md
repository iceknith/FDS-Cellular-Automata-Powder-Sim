# Cellular Automata powder simulator, pour la Fête de la Science A25

Le projet d'Arcadia pour la présentation à la fête de la science A25.

### Infos sur l'Architechture actuelle

Actuellement, seul le sable as été implémenté pour pouvoir tester les capacités du système.   
Mais il est prévu de l'étendre.

Voici une représentation simple du fonctionnement du simulateur sous la forme d'un UML:

<img src="/doc/UML-Projet.png" alt="">

Si vous n'êtes pas très fans (ou mêmes conaisseurs de l'UML, la section ci-dessous explique des trucs)
Si vous avez des questions, hésitez pas à trifouiller le code (normalement il est plutôt éloquent), et n'hésitez pas, non plus à me poser des questions. Je vous répondrait dans les limites de mes dispos.

#### CellularAutomataEngine

Gère la grille de l'automate cellulaire, l'affiche, et gère la mise à jour de chaque cellule.

#### Element

L'élément cellule de base. Il s'agit d'une classe abstraite (c'est à dire qui ne peut être instantiée, et dont le seul but est d'être héritée).
Toute cellule sera un enfant (direct ou non de cette classe).
Elle as quelques attributs, comme:
- Color
- density

Et une fonction:
- update (qui sera override par les différents enfants pour implémenter une logique)

Si vous voulez implémenter un concept de base, qui touchera toutes les cellules, (genre la température, ou autre...) travaillez dans Element.

#### Fils D'élement

Pour avoir un comportement stable selon les différents éléments, je propose que tous les enfants directs d'éléments soient des classes abstraites, qui implémentent différents comportements, mais qui n'ont pas vocation à être implémentés directement (sauf si vous faites un truc qui va être instantié qu'une seule fois).

Donc comme les types de "base" sur lesquels on pourra ensuite travailler, j'avais:
- gaz (vapeur)
- fluides (eau, huile, etc..)
- poudres (sable, dirt, etc...)
- solides (métal (truc qui bougent pas))

Et, finalement, encore, vous pourrez ajouter des trucs ici. Mais globalement, l'idée des fils d'élement, c'est d'implémenter des comportements concrèts, qui pourront ensuite être instantiés dans des classes séparés.

#### Implémentation actuelle
Actuellement, d'implémenté, y'a:
- CellularAutomataEngine
- Element
    - Powder
        - Sand