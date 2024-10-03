package com.example.p01_djpm.ui.theme

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.ui.Alignment
import androidx.compose.runtime.saveable.rememberSaveable
import java.util.Locale

@Composable
fun Screen(modifier: Modifier = Modifier) {

    var displayValue by rememberSaveable { mutableStateOf("0") }
    var firstOperand by remember { mutableDoubleStateOf(0.0) }
    var operation by remember { mutableStateOf<String?>(null) }

    fun updateDisplay(value: String) {
        if (value == "." && displayValue.contains(".")) return
        displayValue = if (displayValue == "0" && value != ".") {
            value
        } else {
            displayValue + value
        }
    }

    fun formatResult(value: Double): String {
        return if (value == value.toInt().toDouble()) {
            value.toInt().toString()
        } else {
            String.format(Locale.US, "%.5f", value)
        }
    }

    fun calculate() {
        val secondOperand = displayValue.toDoubleOrNull() ?: 0.0
        when (operation) {
            "+" -> displayValue = formatResult(firstOperand + secondOperand)
            "-" -> displayValue = formatResult(firstOperand - secondOperand)
            "*" -> displayValue = formatResult(firstOperand * secondOperand)
            "/" -> displayValue = if (secondOperand != 0.0) {
                formatResult(firstOperand / secondOperand)
            } else {
                "Error"
            }
        }
        operation = null
    }

    fun selectOperation(selectedOperation: String) {
        firstOperand = displayValue.toDoubleOrNull() ?: 0.0
        operation = selectedOperation
        displayValue = "0"
    }

    fun backspace() {
        displayValue = if (displayValue.length == 1) {
            "0"
        } else {
            displayValue.dropLast(1)
        }
    }

    Column(
        modifier = modifier
            .fillMaxSize()
            .padding(16.dp),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Spacer(modifier = Modifier.weight(1f))

        Text(
            text = displayValue,
            style = MaterialTheme.typography.headlineLarge.copy(
                fontSize = 48.sp,
                fontWeight = FontWeight.Bold,
                color = Color.Black
            ),
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp),
            maxLines = 1
        )

        Spacer(modifier = Modifier.height(32.dp))

        Column(
            modifier = Modifier.fillMaxWidth(),
            verticalArrangement = Arrangement.Bottom
        ) {
            Row(modifier = Modifier.fillMaxWidth()) {
                CalculatorButton("7", onClick = { updateDisplay("7") }, Modifier.weight(1f).padding(8.dp))
                CalculatorButton("8", onClick = { updateDisplay("8") }, Modifier.weight(1f).padding(8.dp))
                CalculatorButton("9", onClick = { updateDisplay("9") }, Modifier.weight(1f).padding(8.dp))
                CalculatorButton("+", onClick = { selectOperation("+") }, Modifier.weight(1f).padding(8.dp))
            }

            Row(modifier = Modifier.fillMaxWidth()) {
                CalculatorButton("4", onClick = { updateDisplay("4") }, Modifier.weight(1f).padding(8.dp))
                CalculatorButton("5", onClick = { updateDisplay("5") }, Modifier.weight(1f).padding(8.dp))
                CalculatorButton("6", onClick = { updateDisplay("6") }, Modifier.weight(1f).padding(8.dp))
                CalculatorButton("-", onClick = { selectOperation("-") }, Modifier.weight(1f).padding(8.dp))
            }

            Row(modifier = Modifier.fillMaxWidth()) {
                CalculatorButton("1", onClick = { updateDisplay("1") }, Modifier.weight(1f).padding(8.dp))
                CalculatorButton("2", onClick = { updateDisplay("2") }, Modifier.weight(1f).padding(8.dp))
                CalculatorButton("3", onClick = { updateDisplay("3") }, Modifier.weight(1f).padding(8.dp))
                CalculatorButton("*", onClick = { selectOperation("*") }, Modifier.weight(1f).padding(8.dp))
            }

            Row(modifier = Modifier.fillMaxWidth()) {
                CalculatorButton("0", onClick = { updateDisplay("0") }, Modifier.weight(1f).padding(8.dp))
                CalculatorButton("⌫", onClick = { backspace() }, Modifier.weight(1f).padding(8.dp))
                CalculatorButton(".", onClick = { updateDisplay(".") }, Modifier.weight(1f).padding(8.dp))
                CalculatorButton("/", onClick = { selectOperation("/") }, Modifier.weight(1f).padding(8.dp))
            }

            Row(modifier = Modifier.fillMaxWidth()) {
                CalculatorButton("C", onClick = { displayValue = "0" }, Modifier.weight(1f).padding(8.dp))
                CalculatorButton("=", onClick = { calculate() }, Modifier.weight(1f).padding(8.dp))
            }
        }
    }
}

@Composable
fun CalculatorButton(text: String, onClick: () -> Unit, modifier: Modifier = Modifier) {
    Button(
        onClick = onClick,
        shape = CircleShape,
        colors = ButtonDefaults.buttonColors(
            containerColor = Color(0xFF808080),
            contentColor = Color.White
        ),
        modifier = modifier.size(80.dp)
    ) {
        Text(
            text = text,
            fontSize = 24.sp,
            fontWeight = FontWeight.Bold
        )
    }
}

@Preview(showBackground = true)
@Composable
fun ScreenPreview() {
    P01DJPMTheme {
        Screen()
    }
}