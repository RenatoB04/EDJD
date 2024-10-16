const prompt = require('prompt-sync')(); // Para entrada do usuário

// Exercício 1: Definição de Funções e Manipulação de Personagens
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

// Exemplo de uso do Exercício 1
const player1 = createPlayer("Mark", 100, 20);
player1.attack();
player1.takeDamage(30);

// Exercício 2: Inimigos e Objetos no Jogo
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

// Exemplo de uso do Exercício 2
const enemy1 = createEnemy("Goblin", 50, 15);
enemy1.attack();
const loot = enemy1.dropLoot();
console.log(`Loot coletado: ${loot.type} (${loot.amount})`);

// Exercício 3: Arrays e Gestão de Inventário
let inventory = [];

function addItem(item) {
    inventory.push(item);
    console.log(`${item} foi adicionado ao inventário.`);
}

function removeItem(item) {
    const index = inventory.indexOf(item);
    if (index !== -1) {
        inventory.splice(index, 1);
        console.log(`${item} foi removido do inventário.`);
    } else {
        console.log(`${item} não foi encontrado no inventário.`);
    }
}

// Exemplo de uso do Exercício 3
addItem("Espada");
removeItem("Espada");
removeItem("Escudo");

// Exercício 4: Map - Melhorias de Habilidade
const skills = [
    { name: "Espada", level: 1 },
    { name: "Arco", level: 2 },
    { name: "Magia", level: 3 }
];

function increaseSkillLevel(skill) {
    return { name: skill.name, level: skill.level + 1 };
}

const upgradedSkills = skills.map(increaseSkillLevel);
console.log("Habilidades atualizadas:", upgradedSkills);

const abilities = [
    { name: "Força", power: 50 },
    { name: "Agilidade", power: 40 }
];

function increaseAbilityPower(ability) {
    return { name: ability.name, power: ability.power * 1.2 }; // Aumenta o poder em 20%
}

const boostedAbilities = abilities.map(increaseAbilityPower);
console.log("Habilidades impulsionadas:", boostedAbilities);

// Exercício 5: Filter - Filtros de Inventário e Desafios
addItem({ name: "Espada", type: "arma" });
addItem({ name: "Poção", type: "poção" });
addItem({ name: "Moeda", type: "moeda" });
addItem({ name: "Machado", type: "arma" });

const weapons = inventory.filter(item => item.type === "arma");
console.log("Armas no inventário:", weapons);

const challenges = [
    { name: "Desafio 1", difficulty: 5 },
    { name: "Desafio 2", difficulty: 8 },
    { name: "Desafio 3", difficulty: 10 }
];

const hardChallenges = challenges.filter(challenge => challenge.difficulty > 7);
console.log("Desafios difíceis:", hardChallenges);

// Exercício 6: Reduce - Cálculo de Pontuação e Saúde Total
const scores = [10, 20, 30, 40, 50];

function sumScore(total, score) {
    return total + score;
}

const totalScore = scores.reduce(sumScore, 0);
console.log("Pontuação total:", totalScore);

const healthItems = [
    { name: "Poção de Cura", restore: 30 },
    { name: "Erva Medicinal", restore: 20 },
    { name: "Elixir", restore: 50 }
];

function sumHealthRestore(total, item) {
    return total + item.restore;
}

const totalHealthRestore = healthItems.reduce(sumHealthRestore, 0);
console.log("Saúde total restaurada:", totalHealthRestore);

// Exercício 7: Criação de Um Pequeno Sistema de Combate
function combatRound(player, enemy) {
    console.log("---- Início do Combate ----");
    
    while (player.health > 0 && enemy.health > 0) {
        player.attack();
        enemy.takeDamage(player.strength);
        
        if (enemy.health <= 0) {
            console.log(`${enemy.enemyType} foi derrotado! ${player.name} venceu o combate!`);
            return;
        }

        enemy.attack();
        player.takeDamage(enemy.strength);
        
        if (player.health <= 0) {
            console.log(`${player.name} foi derrotado!`);
            return;
        }
    }

    console.log("---- Fim do Combate ----");
}

// Exemplo de uso do Exercício 7
combatRound(player1, enemy1);

// Exercício 8: Eventos Aleatórios e Simulação de Jogo
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

function simulateGame(player) {
    let eventsCount = 0;
    let loot = [];
    let enemiesDefeated = 0;
    console.log(`Início do jogo com ${player.name}! Saúde: ${player.health}`);
    
    while (player.health > 0) {
        prompt('Pressione Enter para continuar...');
        const event = randomEvent();
        eventsCount++;
        console.log(`Evento ${eventsCount}: ${event}`);

        switch (event) {
            case "encontrou um inimigo":
                const enemy = createEnemy("Goblin", 30, 10);
                combatRound(player, enemy);
                
                if (enemy.health <= 0) {
                    enemiesDefeated++;
                    const lootItem = enemy.dropLoot();
                    loot.push(lootItem.type);
                    console.log(`${player.name} coletou um loot: ${lootItem.type} (${lootItem.amount})`);
                }
                break;
            case "achou uma poção":
                const potionHealth = 20;
                player.health += potionHealth;
                loot.push("Poção");
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
    }

    console.log("\n--- Resumo do Jogo ---");
    console.log(`Total de eventos: ${eventsCount}`);
    console.log(`Loot recolhido: ${loot.join(", ")}`);
    console.log(`Inimigos derrotados: ${enemiesDefeated}`);
}

// Exemplo de uso do Exercício 8
simulateGame(player1);