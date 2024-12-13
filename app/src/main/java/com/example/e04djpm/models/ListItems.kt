package com.example.e04djpm.models

data class ListItems (
    var docId: String? = null,
    var name: String? = null,
    var owners: List<String>? = null,
    var checked: Boolean = false
) {
    constructor() : this(null, null, null, false)
}