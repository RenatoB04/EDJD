package com.examples.e02_djpm.models

import org.json.JSONObject
import java.util.Date

class Article(
    var title: String? = null,
    var description: String? = null,
    var urlToImage: String? = null,
    var url: String? = null,
    var publishedAt: Date? = null
) {

    companion object {
        fun fromJson(json: JSONObject): Article {
            val imageUrl = json.optString("multimediaPrincipal", "")

            return Article(
                title = json.optString("titulo", "Sem título"),
                description = json.optString("descricao", "Sem descrição"),
                urlToImage = imageUrl,
                url = json.optString("url", "")
            )
        }
    }
}
