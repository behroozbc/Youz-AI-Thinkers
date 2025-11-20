---
# try also 'default' to start simple
theme: seriph
# random image from a curated Unsplash collection by Anthony
# like them? see https://unsplash.com/collections/94734566/slidev
background: https://cover.sli.dev
# some information about your slides (markdown enabled)
title: Welcome to Slidev
info: |
  ## AI thinkers 
# apply UnoCSS classes to the current slide
class: text-center
# https://sli.dev/features/drawing
drawings:
  persist: false
# slide transition: https://sli.dev/guide/animations.html#slide-transitions
transition: slide-left
# enable MDC Syntax: https://sli.dev/features/mdc
mdc: true
# duration of the presentation
duration: 10min
---

# Mixing Geofence with Semantic search for 


---


# What is Youz?

Youz is a smart, AI-powered app that removes language and information barriers while traveling. Just install the Youz app on your phone, point your camera at a historical monument, temple, museum, scenic site or ask question about a place, and Youz instantly delivers detailed, accurate information about the location in your native language — both as clear audio narration and on-screen text.

--- 

# The main problem

The landmark can be very similar but with different history, usage, city and meaning! which cause simple RAG return inaccurate data! This problem is very critical for your users because we want to help them to understand landmarks. 

---
layout: two-cols
---

<style>
img{
  padding:3px;
  height:300px;
}
</style>
Tehran, Iran
<img src="/tehran.jpg">

::right::
Yazd, Iran
<img src="/Templo_zoroastrista,_Yazd,_Irán,_2016-09-21,_DD_45.jpg">

---

# How solve the problem?

One advantage of these locations is that they are not geographically close to each other, so by knowing this feature, we can use Geo-fence systems to restrict location's data.

<style>
img {
  margin-left:auto;
  margin-right:auto;
}
</style>
<img src="/Picture5.png">

---

# Query step

<v-clicks>

1. Geo-fencing
2. Vector search
</v-clicks>
<v-click>
Make the prompt and get the result!
</v-click>
---
layout: center
---

# Lets see in the code!!!