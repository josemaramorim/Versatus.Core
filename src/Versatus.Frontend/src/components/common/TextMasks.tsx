import React from 'react';
import { IMaskInput } from 'react-imask';

interface MaskProps {
  onChange: (event: { target: { name: string; value: string } }) => void;
  name: string;
}

// 1. CPF Mask: 000.000.000-00
export const CPFMask = React.forwardRef<HTMLInputElement, MaskProps>(
  function CPFMask(props, ref) {
    const { onChange, ...other } = props;
    return (
      <IMaskInput
        {...other}
        mask="000.000.000-00"
        inputRef={ref}
        onAccept={(value: any) => onChange({ target: { name: props.name, value } })}
        overwrite
      />
    );
  }
);

// 2. CNPJ Mask: 00.000.000/0000-00
export const CNPJMask = React.forwardRef<HTMLInputElement, MaskProps>(
  function CNPJMask(props, ref) {
    const { onChange, ...other } = props;
    return (
      <IMaskInput
        {...other}
        mask="00.000.000/0000-00"
        inputRef={ref}
        onAccept={(value: any) => onChange({ target: { name: props.name, value } })}
        overwrite
      />
    );
  }
);

// 3. CEP Mask: 00000-000
export const CEPMask = React.forwardRef<HTMLInputElement, MaskProps>(
  function CEPMask(props, ref) {
    const { onChange, ...other } = props;
    return (
      <IMaskInput
        {...other}
        mask="00000-000"
        inputRef={ref}
        onAccept={(value: any) => onChange({ target: { name: props.name, value } })}
        overwrite
      />
    );
  }
);

// 4. Telefone Mask (Celular/Fixo dinâmico): (00) 0000-0000 ou (00) 00000-0000
export const TelefoneMask = React.forwardRef<HTMLInputElement, MaskProps>(
  function TelefoneMask(props, ref) {
    const { onChange, ...other } = props;
    return (
      <IMaskInput
        {...other}
        mask={[
          { mask: '(00) 0000-0000' },
          { mask: '(00) 00000-0000' }
        ]}
        inputRef={ref}
        onAccept={(value: any) => onChange({ target: { name: props.name, value } })}
        overwrite
      />
    );
  }
);

// 5. Placa de Veículo Mask (Tradicional/Mercosul dinâmico): AAA-9999 ou AAA0A99
export const PlacaMask = React.forwardRef<HTMLInputElement, MaskProps>(
  function PlacaMask(props, ref) {
    const { onChange, ...other } = props;
    return (
      <IMaskInput
        {...other}
        mask={[
          { mask: 'aaa-0000' },
          { mask: 'aaa0a00' }
        ]}
        prepare={(str) => str.toUpperCase()}
        inputRef={ref}
        onAccept={(value: any) => onChange({ target: { name: props.name, value } })}
        overwrite
      />
    );
  }
);
