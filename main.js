// Exercício 1
function createPlayer(name, health, strength) {
    return {
        name: name,
        health: health,
        strength: strength,
        attack: function() {
            console.log(`${this.name} atacou com força de ${this.strength}`);
        },
        takeDamage: function(damage) {
            this.health -= damage;
            if (this.health <= 0) {
                console.log(`${this.name} foi derrotado!`);
            } else {
                console.log(`${this.name} tem agora ${this.health} de saúde.`);
            }
        }
    };
}

// Exercício 2
function createEnemy(enemyType, health, strength) {
    return {
        enemyType: enemyType,
        health: health,
        strength: strength,
        attack: function() {
            console.log(`${this.enemyType} atacou com força de ${this.strength}`);
        },
        takeDamage: function(damage) {
            this.health -= damage;
            if (this.health <= 0) {
                console.log(`${this.enemyType} foi derrotado!`);
            } else {
                console.log(`${this.enemyType} tem agora ${this.health} de saúde.`);
            }
        },
        dropLoot: function() {
            const lootTypes = ["moeda", "poção"];
            const randomType = lootTypes[Math.floor(Math.random() * lootTypes.length)];
            const randomAmount = Math.floor(Math.random() * 50) + 1;
            return { type: randomType, amount: randomAmount };
        }
    };
}

// Exercício 3
function combatRound(player, enemy) {
    console.log("---- Início do Combate ----");
    player.attack();
    enemy.takeDamage(player.strength);
    if (enemy.health <= 0) {
        console.log(`${enemy.enemyType} foi derrotado! ${player.name} venceu o combate!`);
        return;
    }
    enemy.attack();
    player.takeDamage(enemy.strength);
    if (player.health <= 0) {
        console.log(`${player.name} foi derrotado! ${enemy.enemyType} venceu o combate!`);
    } else {
        console.log("O combate continua...");
    }
    console.log("---- Fim do Round ----");
}

// Exercício 4
function randomEvent() {
    const events = [
        "encontrou um inimigo",
        "achou uma poção",
        "ganhou uma moeda",
        "nada aconteceu"
    ];
    const randomIndex = Math.floor(Math.random() * events.length);
    return events[randomIndex];
}

// Exercício 5
function simulateGame(player) {
    let eventsCount = 0;
    let loot = [];
    let enemiesDefeated = 0;
    let healthRestore = 0;
    console.log(`Início do jogo com ${player.name}! Saúde: ${player.health}`);
    while (player.health > 0) {
        const event = randomEvent();
        eventsCount++;
        console.log(`Evento ${eventsCount}: ${event}`);
        switch (event) {
            case "encontrou um inimigo":
                const enemy = createEnemy("Goblin", 30, 10);
                combatRound(player, enemy);
                if (enemy.health <= 0) {
                    enemiesDefeated++;
                    console.log("Inimigo derrotado!");
                    const lootItem = enemy.dropLoot();
                    loot.push(lootItem.type);
                    console.log(`${player.name} coletou um loot: ${lootItem.type} (${lootItem.amount})`);
                }
                break;
            case "achou uma poção":
                const potionHealth = 20;
                player.health += potionHealth;
                loot.push("Poção");
                healthRestore += potionHealth;
                console.log(`${player.name} encontrou uma poção e restaurou ${potionHealth} de saúde! Saúde atual: ${player.health}`);
                break;
            case "ganhou uma moeda":
                loot.push("Moeda");
                console.log(`${player.name} encontrou uma moeda!`);
                break;
            default:
                console.log("Nada aconteceu...");
                break;
        }
        if (player.health <= 0) {
            console.log(`${player.name} foi derrotado!`);
            break;
        }
    }
    console.log("\n--- Resumo do Jogo ---");
    console.log(`Total de eventos: ${eventsCount}`);
    console.log(`Loot recolhido: ${loot.join(", ")}`);
    console.log(`Inimigos derrotados: ${enemiesDefeated}`);
    console.log(`Saúde restaurada: ${healthRestore}`);
}

// Exercício 6
function sumScore(total, score) {
    return total + score;
}

// Exercício 7
function sumHealthRestore(total, item) {
    return total + item.restore;
}

// Exercício 8
const player1 = createPlayer("Mark", 100, 20);
const scores = [10, 20, 30, 40, 50];
const totalScore = scores.reduce(sumScore, 0);
console.log("Pontuação total:", totalScore);

const healthItems = [
    { name: "Poção de Cura", restore: 30 },
    { name: "Erva Medicinal", restore: 20 },
    { name: "Elixir", restore: 50 }
];
const totalHealthRestore = healthItems.reduce(sumHealthRestore, 0);
console.log("Saúde total restaurada:", totalHealthRestore);

// Simulação de jogo
simulateGame(player1);