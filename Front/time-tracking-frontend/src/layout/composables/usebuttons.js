// useButtonToggle.js
import { ref } from 'vue';

export function useButtonToggle() {
  const showButton1 = ref(true);
  const showButton2 = ref(false);

  const toggleButtons = () => {
    console.log('Toggling buttons - Current state: showButton1:', showButton1.value, 'showButton2:', showButton2.value);
    if (showButton1.value) {
      showButton1.value = false;
      showButton2.value = true;
    } else if (showButton2.value) {
      showButton2.value = false;
      showButton1.value = true;
    }
  };

  return { showButton1, showButton2, toggleButtons };
}