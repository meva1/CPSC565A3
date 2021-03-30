# CPSC565 Assignment 3

UNITY VERSION: 2019.2.5f1

![A3example2](https://user-images.githubusercontent.com/41522967/112935990-2accb700-90e2-11eb-8349-f1d2caa2404c.jpg)

## Introduction
This assignment involved extending the world and terrain located at https://github.com/DaviesCooper/Antymology to create a colony of ants capable of maximizing nest block production. The ant colony consists of some number of ants (100 by default) and a Queen. The ants are controlled by the script AntBehaviour and the Queen by QueenBehaviour. Initialization of the ants and Queen is handled in the WorldManager script. There is also ScoreScript to display the number of nest blocks produced and a slightly modified FlyCamera. The camera is control with 'wasd' keys and can pan around when holding down left click. The ants appear as blue cubes in the above picture and the Queen is teal. 

## AntBehaviour
Ants start with 1000 health and lose 1 health per frame, or 2 if standing on an AcidBlock. When their health reaches 0 they die and are removed from the simulation, never to be replaced. The primary purpose of the ants is to donate their health to the queen. When occupying the same block as the queen ants will donate 75% of their health, or less if they would fill the Queens health above maximum. Ants default mode is to move slightly randomly but with moves that place them closer to the Queen having a slight preference. This is signified by the boolean variable moveToQueen. Upon reaching the Queen they donate their health and begin moving randomly but not with a preference to move further from the Queen, the idea being that they will go find a MulchBlock, consume it to refill their health, and upon consuming a MulchBlock being set to move towards the Queen again. Only moves directly to the north, south, east or west are considered and only moves to blocks up to 2 levels higher or lower are possible.

## QueenBehaviour
Like ants, the Queen starts with 1000 health losing 1 per frame or 2 when on an AcidBlock. The Queen's primary responsibility is to place NestBlocks. Placing a NestBlock costs the Queen 1/3rd of it's maximum health so the Queen relies on the ants donating their health and consumption of MulchBlocks to refill its own health. The movement of the Queen is more complicated because if the Queen ever gets itself stuck it can no longer place NestBlocks. There are methods to prevent the Queen from moving into a hole and if the Queen ever gets stuck up too high it will dig down. Other than to prevent the Queen from getting stuck it moves completely randomly. This random movement helps to keep the Queen from building itself into a corner with NestBlocks.

## WorldManager
The initial ant and Queen generation is handled by WorldManager. Every 1000 frames the least fit 5% of ants based on health and also 5% least fit based on largest distance to the Queen are removed and replaced by the same number of randomly placed ants. 

## Expected Behaviour
The simulation is typically fairly successful, generating 2000-4000 NestBlocks starting with only 100 ants. The ants that die from their health being reduced to 0 are never replaced so the ant population is in continual slow decline. Eventually there are not enough ants to donate health to the Queen and the Queen is unable to find enough MulchBlocks so the Queen will die and the simulation will end. Smaller amounts of ants tend to maximize NestBlock production as too many ants will lead to inefficient consumption of MulchBlocks and crowding out of the Queen leaving it unable to consume a MulchBlock when another ant is on that block.
