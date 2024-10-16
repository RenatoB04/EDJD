function calculateAverageScore(scores) {
    const totalScore = scores.reduce((acc, score) => acc + score, 0);
    return totalScore / scores.length;
}

const dolphinsScores = [96, 108, 89];
const koalasScores = [88, 91, 110];

const dolphinsAverage = calculateAverageScore(dolphinsScores);
const koalasAverage = calculateAverageScore(koalasScores);

console.log(`Dolphins' Average Score: ${dolphinsAverage.toFixed(2)}`);
console.log(`Koalas' Average Score: ${koalasAverage.toFixed(2)}`);

if (dolphinsAverage >= 100 && koalasAverage >= 100) {
    if (dolphinsAverage > koalasAverage) {
        console.log("Dolphins win the trophy!");
    } else if (koalasAverage > dolphinsAverage) {
        console.log("Koalas win the trophy!");
    } else {
        console.log("It's a draw! Both teams have the same average score.");
    }
} else {
    console.log("No team wins the trophy, as both teams must have an average score of at least 100.");
}

const dolphinsScoresBonus1 = [97, 112, 101];
const koalasScoresBonus1 = [109, 95, 123];

const dolphinsAverageBonus1 = calculateAverageScore(dolphinsScoresBonus1);
const koalasAverageBonus1 = calculateAverageScore(koalasScoresBonus1);

console.log(`\nBonus Test Data 1 - Dolphins' Average Score: ${dolphinsAverageBonus1.toFixed(2)}`);
console.log(`Bonus Test Data 1 - Koalas' Average Score: ${koalasAverageBonus1.toFixed(2)}`);

if (dolphinsAverageBonus1 >= 100 && koalasAverageBonus1 >= 100) {
    if (dolphinsAverageBonus1 > koalasAverageBonus1) {
        console.log("Dolphins win the trophy!");
    } else if (koalasAverageBonus1 > dolphinsAverageBonus1) {
        console.log("Koalas win the trophy!");
    } else {
        console.log("It's a draw! Both teams have the same average score.");
    }
} else {
    console.log("No team wins the trophy, as both teams must have an average score of at least 100.");
}

const dolphinsScoresBonus2 = [97, 112, 101];
const koalasScoresBonus2 = [109, 95, 106];

const dolphinsAverageBonus2 = calculateAverageScore(dolphinsScoresBonus2);
const koalasAverageBonus2 = calculateAverageScore(koalasScoresBonus2);

console.log(`\nBonus Test Data 2 - Dolphins' Average Score: ${dolphinsAverageBonus2.toFixed(2)}`);
console.log(`Bonus Test Data 2 - Koalas' Average Score: ${koalasAverageBonus2.toFixed(2)}`);

if (dolphinsAverageBonus2 >= 100 && koalasAverageBonus2 >= 100) {
    if (dolphinsAverageBonus2 > koalasAverageBonus2) {
        console.log("Dolphins win the trophy!");
    } else if (koalasAverageBonus2 > dolphinsAverageBonus2) {
        console.log("Koalas win the trophy!");
    } else {
        console.log("It's a draw! Both teams have the same average score.");
    }
} else {
    console.log("No team wins the trophy, as both teams must have an average score of at least 100.");
}
