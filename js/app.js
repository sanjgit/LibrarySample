import { BookDetails } from "./search.js";
import SimpleStore from './simpleStore.js'

class App
{
	
	constructor() {
		// setup
    }

	go() {
		var bookDetails = new BookDetails();
		const refs = {
			list: document.querySelector('#Search-list'),
			form: document.querySelector('#Search-Form'),
			input: document.querySelector('input'),
			button: document.querySelector('button'),
		};
		var list = document.querySelector('#My-Books');
		var booksList = []
		//list click event for books
		list.addEventListener('click', function (ev) {			
			const active = document.querySelector('.checked');
			if (active) {
				active.classList.remove('checked');
			}
			if (ev.target.tagName === 'LI') {
				for (let i = 0; i < booksList.length; i++) {
					if (booksList[i].Title == ev.target.textContent) {
						new SimpleStore({ id: booksList[i].Id });
						break;
                    }
                }
				ev.target.classList.toggle('checked');
				bookDetails.loadTopCommonWords();
			}
		}, false);
		refs.input.addEventListener('input', bookDetails.catchInput);

		fetch('api/books').then(function (response) {
			// The API call was successful!
			return response.json();
		}).then(function (data) {
			// This is the JSON from our response
			//var books = response.json();
			booksList = data;
			for (let i = 0; i < data.length; i++) {
				var li = document.createElement("li");
				var inputValue = data[i].Title;
				var t = document.createTextNode(inputValue);
				li.appendChild(t);
				document.getElementById("My-Books").appendChild(li);
			}
		}).catch(function (err) {
			// There was an error
			console.warn('Something went wrong.', err);
		});

	}
	
}
new App().go();