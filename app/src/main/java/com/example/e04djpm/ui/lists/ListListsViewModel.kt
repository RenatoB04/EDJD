package com.example.e04djpm.ui.lists

import android.util.Log
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import com.google.firebase.firestore.ktx.firestore
import com.google.firebase.ktx.Firebase
import com.example.e04djpm.TAG
import com.example.e04djpm.models.ListItems

data class ListListsState(
    val listItemsList: List<ListItems> = arrayListOf(),
    val isLoading: Boolean = false,
    val error: String? = null
)

class ListListsViewModel : ViewModel() {

    var state = mutableStateOf(ListListsState())
        private set

    fun getLists() {
        val db = Firebase.firestore

        db.collection("lists")
            .get()
            .addOnSuccessListener { documents ->
                val listItemsList = arrayListOf<ListItems>()
                for (document in documents) {
                    Log.d(TAG, "${document.id} => ${document.data}")
                    val listItem = document.toObject(ListItems::class.java)
                    listItem.docId = document.id
                    listItemsList.add(listItem)
                }
                state.value = state.value.copy(
                    listItemsList = listItemsList
                )
            }
            .addOnFailureListener { exception ->
                Log.w(TAG, "Error getting documents: ", exception)
            }
    }

    fun removeItem(itemId: String) {
        val db = Firebase.firestore
        db.collection("lists")
            .document(itemId)
            .delete()
            .addOnSuccessListener {
                Log.d(TAG, "Item removed successfully")
                getLists()
            }
            .addOnFailureListener { e ->
                Log.e(TAG, "Error removing item", e)
            }
    }

    fun updateItemStatus(itemId: String, isChecked: Boolean) {
        val db = Firebase.firestore
        db.collection("lists")
            .document(itemId)
            .update("checked", isChecked)
            .addOnSuccessListener {
                Log.d(TAG, "Item status updated successfully")
                getLists()
            }
            .addOnFailureListener { e ->
                Log.e(TAG, "Error updating item status", e)
            }
    }
}