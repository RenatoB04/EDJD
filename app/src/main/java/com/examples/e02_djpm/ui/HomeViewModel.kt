package com.examples.e02_djpm.ui

import android.util.Log
import androidx.lifecycle.ViewModel
import com.examples.e02_djpm.models.Article
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import okhttp3.Call
import okhttp3.Callback
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.Response
import org.json.JSONObject
import java.io.IOException
import org.json.JSONArray


data class ArticlesState(
    val articles: ArrayList<Article> = arrayListOf(),
    val isLoading: Boolean = false,
    val error: String? = null
)

class HomeViewModel : ViewModel() {

    private val _uiState = MutableStateFlow(ArticlesState())
    val uiState: StateFlow<ArticlesState> = _uiState.asStateFlow()

    fun fetchArticles() {

        _uiState.value = ArticlesState(
            isLoading = true,
            error = null
        )

        val client = OkHttpClient()

        val request = Request.Builder()
            .url("https://www.publico.pt/api/list/ultimas")
            .build()

        client.newCall(request).enqueue(object : Callback {
            override fun onFailure(call: Call, e: IOException) {
                e.printStackTrace()
                Log.e("HomeViewModel", "API request failed: ${e.message}")
                _uiState.value = ArticlesState(
                    isLoading = false,
                    error = e.message
                )
            }

            override fun onResponse(call: Call, response: Response) {
                response.use {
                    if (!response.isSuccessful) {
                        Log.e("HomeViewModel", "Unexpected code $response")
                        _uiState.value = ArticlesState(
                            isLoading = false,
                            error = "Erro na resposta da API: $response"
                        )
                        return
                    }

                    val articlesResult = arrayListOf<Article>()
                    val result = response.body!!.string()
                    val jsonArray = JSONArray(result)
                    for (index in 0 until jsonArray.length()) {
                        val articleJson = jsonArray.getJSONObject(index)

                        val title = articleJson.optString("titulo", "Sem título")
                        val url = articleJson.optString("url", "")
                        val description = articleJson.optString("descricao", "Sem descrição")
                        val imageUrl = articleJson.optString("multimediaPrincipal", "")

                        val article = Article(
                            title = title,
                            description = description,
                            url = url,
                            urlToImage = imageUrl
                        )

                        Log.d("HomeViewModel", "Título: $title, Descrição: $description, Imagem: $imageUrl")
                        articlesResult.add(article)
                    }

                    _uiState.value = ArticlesState(
                        articles = articlesResult,
                        isLoading = false,
                        error = null
                    )
                }
            }
        })
    }
}