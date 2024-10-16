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

const player1 = createPlayer("Mark", 100, 20);
player1.attack();
player1.takeDamage(30); 
player1.takeDamage(80); 