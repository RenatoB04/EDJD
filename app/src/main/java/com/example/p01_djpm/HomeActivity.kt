package com.example.p01_djpm

import android.content.Intent
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.example.p01_djpm.databinding.ActivityHomeBinding

class HomeActivity : AppCompatActivity() {
    private lateinit var binding: ActivityHomeBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        binding = ActivityHomeBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.addBookButton.setOnClickListener {
            val intent = Intent(this, AddBookActivity::class.java)
            startActivity(intent)
        }
    }
}