<script>
import axios from 'axios'
import logo from "./assets/ec-locator-logo.png"


export default {
  name: 'app',
  data() {
    return {
      logo: logo,
      users: [],
      selectedUser: null,
      highlightedIndex: -1,
      showUntil: false,
      location: {},
      input: "",

      usersTest: [
        { "name": "Anders Wind", "id": "2cf3e351-6ca8-4fda-999c-14a8b048b899" },
        { "name": "Andreas Widtfeldt Trolle", "id": "2a6f1d7f-d155-40e7-b2db-e4723ea77c0f" },
        { "name": "Brian U. B. Daugaard", "id": "2d3cfcdf-542d-43f5-a4b1-6f58387604eb" },
        { "name": "Casper Stendal", "id": "16a2276a-ba4f-40e0-82ab-d3fe7d848cdb" },
        { "name": "christian@bangbove.dk", "id": "08784676-7ff1-4f30-839d-8121b2396327" }],

      locationTest: {
        place: "remote",
        locationEndTime: "14:30",
        teamMessage: "Jeg er til møde indtil 14:30",
        calendarInfo: "møde med meget vigtig kunde 9-14:30"
      }
    }
  },
  methods: {
    async getLocation() {
      //this.location = this.locationTest
      //return
      await axios.get('https://localhost:7208/Location/' + this.selectedUser.id).then((res) => {
        console.log(res.data)
        this.location = res.data
      })
    },
    updateSelected(selected) {
      console.log(selected.id)
      this.selectedUser = selected
      this.input = this.selectedUser.name
      this.$refs.textInput.blur();
      this.getLocation()
    },
    filteredUsers() {
      if (this.input.length == 0) return null
      return this.users.filter(user => user.name.toLowerCase().startsWith(this.input.toLowerCase()))
    },
    enterHandler() {
      console.log(this.filteredUsers().length)
      if (this.filteredUsers().length == 1) {
        this.updateSelected(this.filteredUsers()[0])
      }
      if (this.highlightedIndex != -1) {
        this.updateSelected(this.filteredUsers()[this.highlightedIndex])
      }
    },
    highlightNext() {
      if (this.filteredUsers != null) {
        if (this.highlightedIndex < this.filteredUsers().length - 1) {
          this.highlightedIndex++
        }
      }
    },
    highlightPrevious() {
      if (this.highlightedIndex > -1)
        this.highlightedIndex--
    },
    resetHighlight() {
      this.highlightedIndex = -1
      this.selectedUser = null;
    },
    renderPlace(place) {
      if (place === "off") {
        this.showUntil = false;
        return "holder fri"
      }
      if (place === "office") {
        this.showUntil = true;
        return "er på kontoret"
      }
      if (place === "ill") {
        this.showUntil = false;
        return "er syg"
      }
      if (place === "meeting") {
        this.showUntil = true;
        return "er til møde"
      }
      if (place === "home") {
        this.showUntil = true;
        return "arbejder hjemmefra"
      }
      if (place === "KidsIll") {
        this.showUntil = false;
        return "har syge børn"
      }
      if (place === "remote") {
        this.showUntil = true;
        return "er hos en kunde"
      }
      if (place === "undefined") {
        this.showUntil = false;
        return "kan ikke lokaliseres ud fra besked"
      }
    },
    clearInputAndFocus() {
      this.input = '';
      this.selectedUser = null;
      this.$refs.textInput.focus();
    },
    onKeyDown(event) {
      if (event.keyCode === 8 && document.activeElement !== this.$refs.textInput){
        {
          this.clearInputAndFocus()
        }
      }
    }
  },


  mounted() {
    
        axios.get('https://localhost:7208/Users').then((res) => {
          this.users = res.data
        })

        
        document.addEventListener('keydown', this.onKeyDown);


    //this.users = this.usersTest
  }
  
}
</script>

<template>

  <header>
    <div>
      <img :src="logo" alt="EC-Locator" class="img-fluid" />
    </div>
  </header>

  <main>

    <div class="container">

      <div class="row">

        <div class="col-2"></div>
        <div class="col-8">


        <div class="input-group">
          <input type="text" class="form-control input-field" 
            v-model="input" v-on:input="resetHighlight()"
            placeholder="Search employee..." aria-describedby="basic-addon2" 
            @keyup.enter="enterHandler()"
            @keydown.down.prevent="highlightNext()" 
            @keydown.up.prevent="highlightPrevious()" 
            ref="textInput">
          <div class="input-group-append">
              <button class="btn btn-secondary" type="button" 

            @click="clearInputAndFocus()">Clear</button>
            </div>
        </div>

        <table v-if="selectedUser == null" class="row user-table">
          <tbody>
            <tr v-for="(user, index) in filteredUsers()" :key="user.name" @click="updateSelected(user)" @mouseover="">
              <td class="user" v-if="index === highlightedIndex" style="font-weight: bold;">{{ user.name }}</td>
              <td v-else class="user">{{ user.name }} </td>
            </tr>
          </tbody>
        </table>
      </div>

        <div class="col-2"></div>
        <div v-if="selectedUser != null" class="present-location">
          <div class="border rounded border-3 top-box">
          <p v-if="showUntil==true" class="center-vertical">{{ selectedUser.name }} {{ renderPlace(location.place) }} indtil kl. {{ location.locationEndTime }}</p>
          <p v-if="showUntil==false" class="center-vertical">{{ selectedUser.name }} {{ renderPlace(location.place) }} </p>
        </div>

        <div class="border rounded border-2 present-location">
          <p>Messages</p>
          <p class="indent-text" >{{ location.teamMessage }}</p>
        </div>

        <div class="border rounded border-2 present-location">
          <p>Calendar</p>
          <p class="indent-text">{{ location.calendarInfo }}</p>
        </div>

        
        


        </div>

      </div>
    </div>
  </main>
</template>