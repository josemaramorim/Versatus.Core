import { CustomThemeProvider } from './context/ThemeContext';
import { MenuProvider } from './context/MenuContext';
import { TabsProvider } from './context/TabsContext';
import { AppShell } from './components/layout/AppShell';

function App() {
  return (
    <CustomThemeProvider>
      <MenuProvider>
        <TabsProvider>
          <AppShell />
        </TabsProvider>
      </MenuProvider>
    </CustomThemeProvider>
  );
}

export default App;
