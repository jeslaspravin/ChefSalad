# ChefSalad :

# About :
  This is a 2D two player couch PvP or Co-op Salad restaurant Simulation game.Both players will be controlling their character from a keyboard.<br>
  The goal of the game is serve the cutomers with the salad of their requested vegetables as ingredients before the customer loss their patience and leave the restaurant.
  
# Gameplay :
  * Controls W,A,S,D for Player 1,Arrow keys for Player 2.
  * Player has vegetable tables in both edges of level from where they can pick vegetables that are needed for preparing salad.
  * Each player will have a timer that when runs out player will not be able to serve any more customers.
  * Each player on serving a customer will be awarded points based on ingredients of the salad and slightly based on customer mood(So it depends on customer too).
  * If the customer get serviced before losing their 70% of patience they provide one of three special items that gets spawned in world for only the player that served the customer can collect it.
  * Player can trash the vegetables or salad that they prepared in trash bin if they want so, Of course they have to pay with their score for what ever they trash.
  * Inventory items works in order of first picked item gets used/dropped first.Except in situation when you are serving your customer, When even if you have vegetable before prepared salad you will automatically serve the salad to customer.
  * Each chef(Player) has their own stove/table to prepare salad and have their own dish where they can place buffer vegetable for quick access.
  * If customer leaves after losing patience without being served both the players get penaltly score.
  * If you provide salad with wrong combination to customer,The customer gets angry and starts losing patience faster.If the customer leave without being fed with their preferred salad,only the chefs whoever provided incorrect salad gets 2 times the score penalty.
  * Once both the players timer runs out the game ends and winner will be annouced with top 10 highscores previously scored.
  * Collectibles that customer provide :
      * Score booster - Gives additional score to player.
      * Time booster - Adds few seconds of time to player timer.
      * Speed booster - Increases player's movement speed for certain amount of time.
      
# Assumptions Made :
  * The Code implementations were made by assuming that customer only ask for unique vegetables in their salad ingredients.
  * Restricted player to camera field of view zone and also spawns the special collectibles only in player reachable zone.
  * Interaction system picks up and drops vegetables and salad automatically. This is done in point of view that since both player uses same keyboard the space becomes limit and so wanted to keep the keys player use to as minimal as possible.
  * Since interaction system is automatic it is assumed if player has vegetable in top of their inventory,It gets automatically added to salad that is being already prepared in stove.
  * If player has two vegetables in inventory,then all vegetables are automatically added to salad as long as salad's number of ingredient does not crossed max allowed.
  * Assumed that at max only 3 ingredients are allowed in salad (Can be increased by changing ChefSaladManager.MAX_INGREDIENT_COUNT)

# Next Step :
  * Making player mobility zone unrestricted , Make camera zoom out so that player gets unrestricted feel and also level can be expanded however we want.
  *	More visual feedback to player,Like showing angry customer so player know it too.
  * Make spawned special items color coded or in some way so that player can distinguise collectibles that belong to them.
  * Right now all the interaction are automatic and trigger based so after preparing salad player has to reenter the trigger to collect salad automatically,This might suit some players. So based on feed back another layer of interaction needs to be added to let player control what they pickup and what they don't.
  *	Most of the UI related,NPC pawn spawning is pooled but collectibles spawning are not pooled,Have to make those spawning with object pooling as well.
  *	Now pause,unpause and save load systems are in ChefSaladManager,This should be made into seperate module if requirement for those similar feature increases.
  * Create a proper buffs and effects framework so that it works generically and can be useful to extend with many more collectibles.
