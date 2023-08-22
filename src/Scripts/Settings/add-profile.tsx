import React from 'react';
import {useState} from 'react';
import {createRoot} from 'react-dom/client';

const container = document.getElementById('add-profile-modal');
const root = createRoot(container!); // createRoot(container!) if you use TypeScript
root.render(<AddProfileModal />);


function AddProfileModal() {
	let [counter, setCounter] = useState(0);
	const handleClick = () => setCounter(counter +1);
	return (
		<>
			<div>
				<button className="btn btn-primary" onClick={handleClick}>Click here</button>
			</div>
			<div>{counter}</div>
		</>
);
}