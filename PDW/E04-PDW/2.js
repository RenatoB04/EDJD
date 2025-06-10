function calculateBMI(mass, height) {
    return mass / (height * height);
}

const markMass1 = 78;
const markHeight1 = 1.69;
const johnMass1 = 92;
const johnHeight1 = 1.95;

const markBMI1 = calculateBMI(markMass1, markHeight1);
const johnBMI1 = calculateBMI(johnMass1, johnHeight1);

if (markBMI1 > johnBMI1) {
    console.log(`Mark's BMI (${markBMI1.toFixed(2)}) is higher than John's (${johnBMI1.toFixed(2)})!`);
} else if (johnBMI1 > markBMI1) {
    console.log(`John's BMI (${johnBMI1.toFixed(2)}) is higher than Mark's (${markBMI1.toFixed(2)})!`);
} else {
    console.log(`Mark and John have the same BMI (${markBMI1.toFixed(2)})!`);
}

const markMass2 = 95;
const markHeight2 = 1.88;
const johnMass2 = 85;
const johnHeight2 = 1.76;

const markBMI2 = calculateBMI(markMass2, markHeight2);
const johnBMI2 = calculateBMI(johnMass2, johnHeight2);

if (markBMI2 > johnBMI2) {
    console.log(`\nMark's BMI (${markBMI2.toFixed(2)}) is higher than John's (${johnBMI2.toFixed(2)})!`);
} else if (johnBMI2 > markBMI2) {
    console.log(`\nJohn's BMI (${johnBMI2.toFixed(2)}) is higher than Mark's (${markBMI2.toFixed(2)})!`);
} else {
    console.log(`\nMark and John have the same BMI (${markBMI2.toFixed(2)})!`);
}
